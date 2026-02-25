using System;
using System.Text;
using System.Transactions;

using namasdev.Core.Exceptions;
using namasdev.Core.Transactions;
using namasdev.Core.Validation;

using MyApp.Entities;
using MyApp.Entities.Values;
using MyApp.Data;

namespace MyApp.Business
{
    public interface IErrorsBusiness
    {
        string AddAndGetUserFriendlyMessage(Exception ex, params object[] arguments);
        string AddAndGetUserFriendlyMessage(Exception ex, string userId, params object[] arguments);
    }

    public class ErrorsBusiness : IErrorsBusiness
    {
        private IErrorsRepository _errorsRepository;
        private IParametersRepository _parametersRepository;

        public ErrorsBusiness(IErrorsRepository errorsRepository, IParametersRepository parametersRepository)
        {
            Validator.ValidateRequiredArgumentAndThrow(errorsRepository, nameof(errorsRepository));
            Validator.ValidateRequiredArgumentAndThrow(parametersRepository, nameof(parametersRepository));

            _errorsRepository = errorsRepository;
            _parametersRepository = parametersRepository;
        }

        public string AddAndGetUserFriendlyMessage(Exception ex, params object[] arguments)
        {
            return AddAndGetUserFriendlyMessage(ex, null, arguments);
        }

        public string AddAndGetUserFriendlyMessage(Exception ex, string userId,
            params object[] arguments)
        {
            using (var ts = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
            {
                string defaultMessage = _parametersRepository.Get(Parameters.ERRORS_DEFAULT_MESSAGE);

                try
                {
                    if (ex == null)
                        throw new ArgumentNullException("ex");

                    string exMessage = ex.Message;

                    var exUserException = ex as ExceptionFriendlyMessage;
                    if (exUserException != null)
                    {
                        defaultMessage = exUserException.Message;
                        exMessage = exUserException.InternalMessage;

                        if (!exUserException.MustLogError)
                        {
                            return defaultMessage;
                        }
                    }

                    if (exMessage.Contains(defaultMessage))
                        return defaultMessage;

                    var sbArguments = new StringBuilder();
                    int argsLen = arguments.Length;
                    for (int i = 0; i < argsLen; i++)
                    {
                        object argument = arguments[i];
                        string argValue = string.Empty;
                        if (argument != null)
                        {
                            if (argument is byte[])
                                argValue = "byte[" + ((byte[])argument).Length + "]";
                            else
                                argValue = argument.ToString();
                        }

                        sbArguments.Append("[" + argValue + "];");
                    }

                    _errorsRepository.Add(
                        new Error
                        {
                            Id = Guid.NewGuid(),
                            Message = ExceptionHelper.GetMessagesRecursively(ex),
                            Source = ex.Source,
                            StackTrace = ex.StackTrace,
                            Arguments = sbArguments.ToString().TrimEnd(';'),
                            DateTime = DateTime.Now,
                            UserId = userId
                        });

                    ts.Complete();
                }
                catch
                {
                }

                return defaultMessage;
            }
        }
    }
}
