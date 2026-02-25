using System;
using System.Collections.Generic;

using AutoMapper;
using namasdev.Core.Entity;
using namasdev.Core.Transactions;
using namasdev.Core.Validation;

using MyApp.Entities;
using MyApp.Entities.Metadata;
using MyApp.Data;
using MyApp.Business.DTO.Users;

namespace MyApp.Business
{
    public interface IUsersBusiness
    {
        User Add(AddParameters parameters);
        void Update(UpdateParameters parameters);
        void SetDeleted(SetDeletedParameters parameters);
        void UnsetDeleted(UnsetDeletedParameters parameters);
        User Get(string id, bool validateExists = true);
    }

    public class UsersBusiness : BusinessBase<IUsersRepository>, IUsersBusiness
    {
        private readonly IAspNetIdentityRepository _identityRepository;

        public UsersBusiness(IAspNetIdentityRepository identityRepository, 
            IUsersRepository usersRepository, IErrorsBusiness errorsBusiness, IMapper mapper)
            : base(usersRepository, errorsBusiness, mapper)
        {
            Validator.ValidateRequiredArgumentAndThrow(identityRepository, nameof(identityRepository));
            
            _identityRepository = identityRepository;
        }

        public User Add(AddParameters parameters)
        {
            Validator.ValidateRequiredArgumentAndThrow(parameters, nameof(parameters));

            DateTime now = DateTime.Now;

            var user = Mapper.Map<User>(parameters);
            user.Id = Guid.NewGuid().ToString();
            user.SetCreated(parameters.LoggedUserId, now);
            user.SetModified(parameters.LoggedUserId, now);

            ValidateUser(user);

            using (var ts = TransactionScopeFactory.Create())
            {
                Repository.Add(user);
                _identityRepository.AddOrUpdateUsersIdentity(user.Id, parameters.Role);

                ts.Complete();
            }

            return user;
        }

        public void Update(UpdateParameters parameters)
        {
            Validator.ValidateRequiredArgumentAndThrow(parameters, nameof(parameters));

            var user = Get(parameters.Id);
            Mapper.Map(parameters, user);
            user.SetModified(parameters.LoggedUserId, DateTime.Now);

            ValidateUser(user);

            using (var ts = TransactionScopeFactory.Create())
            {
                Repository.Update(user);
                _identityRepository.AddOrUpdateUsersIdentity(user.Id, parameters.Role);

                ts.Complete();
            }
        }

        public void SetDeleted(SetDeletedParameters parameters)
        {
            Validator.ValidateRequiredArgumentAndThrow(parameters, nameof(parameters));

            var user = Mapper.Map<User>(parameters);
            user.SetDeleted(parameters.LoggedUserId, DateTime.Now);

            using (var ts = TransactionScopeFactory.Create())
            {
                Repository.UpdateDeletedProperties(user);
                _identityRepository.AddOrUpdateUsersIdentity(user.Id);

                ts.Complete();
            }
        }

        public void UnsetDeleted(UnsetDeletedParameters parameters)
        {
            Validator.ValidateRequiredArgumentAndThrow(parameters, nameof(parameters));

            var user = Mapper.Map<User>(parameters);
            
            using (var ts = TransactionScopeFactory.Create())
            {
                Repository.UpdateDeletedProperties(user);
                _identityRepository.UnlockUser(user.Id);

                ts.Complete();
            }
        }

        public User Get(string id,
            bool validateExists = true)
        {
            var user = Repository.Get(id);
            if (validateExists
                && user == null)
            {
                throw new Exception(Validator.Messages.EntityNotFound(UserMetadata.LABEL, id));
            }

            return user;
        }

        private void ValidateUser(User user)
        {
            var errors = new List<string>();

            Validator.ValidateEmailAndAddToErrorList(user.Email, UserMetadata.Properties.Email.LABEL, required: true, errors);
            Validator.ValidateStringAndAddToErrorList(user.FirstName, UserMetadata.Properties.FirstName.LABEL, required: true, errors, maxLength: UserMetadata.Properties.FirstName.LENGTH_MAX);
            Validator.ValidateStringAndAddToErrorList(user.LastName, UserMetadata.Properties.LastName.LABEL, required: true, errors, maxLength: UserMetadata.Properties.LastName.LENGTH_MAX);

            Validator.ThrowExceptionFriendlyMessageIfAnyErrors(errors);
        }
    }
}
