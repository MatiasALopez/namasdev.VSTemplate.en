using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Text;

using AutoMapper;
using namasdev.Core.Exceptions;
using namasdev.Core.Validation;
using namasdev.Net.Mail;

using MyApp.Entities;
using MyApp.Entities.Metadata;
using MyApp.Entities.Values;
using MyApp.Data;
using MyApp.Business.DTO.Emails;

namespace MyApp.Business
{
    public interface IEmailsBusiness
    {
        void SendActivateAccount(SendActivateAccountParameters parameters);
        void SendResetPassword(SendResetPasswordParameters parameters);
        void Send(SendWithSubjectAndBodyParameters parameters);
        void Send(SendWithIdParameters parameters);
        void Send(SendWithParametersParameters parameters);
    }

    public class EmailsBusiness : BusinessBase<IEmailsParametersRepository>, IEmailsBusiness
    {
        private readonly IEmailServer _emailServer;

        public EmailsBusiness(IEmailServer emailServer, IEmailsParametersRepository emailsParametersRepository, IErrorsBusiness errorsBusiness, IMapper mapper)
            : base(emailsParametersRepository, errorsBusiness, mapper)
        {
            Validator.ValidateRequiredArgumentAndThrow(emailServer, nameof(emailServer));

            _emailServer = emailServer;
        }

        public void SendActivateAccount(SendActivateAccountParameters parameters)
        {
            Send(new SendWithIdParameters
            {
                To = parameters.Email,
                EmailParametersId = MailsParameters.ACTIVATE_ACCOUNT,
                BodyVariables = new Dictionary<string, string>
                {
                    { "FullName", parameters.FullName },
                    { "ActivateAccountUrl", parameters.ActivateAccountUrl },
                },
            });
        }

        public void SendResetPassword(SendResetPasswordParameters parameters)
        {
            Send(new SendWithIdParameters
            {
                To = parameters.Email,
                EmailParametersId = MailsParameters.RESET_PASSWORD,
                BodyVariables = new Dictionary<string, string>
                {
                    { "FullName", parameters.FullName },
                    { "ResetPasswordUrl", parameters.ResetPasswordUrl },
                },
            });
        }
        
        public void Send(SendWithIdParameters parameters)
        {
            var p = Mapper.Map<SendWithParametersParameters>(parameters);
            p.Parameters = GetEmailParameters(parameters.EmailParametersId);
            Send(p);
        }

        public void Send(SendWithSubjectAndBodyParameters parameters)
        {
            var p = Mapper.Map<SendWithParametersParameters>(parameters);
            p.Parameters = new EmailParameters
            {
                Subject = parameters.Subject,
                Content = parameters.Content,
            };

            Send(p);
        }

        public void Send(SendWithParametersParameters parameters)
        {
            using (var correo = CreateMailMessage(parameters))
            {
                _emailServer.SendMail(correo);
            }
        }

        private EmailParameters GetEmailParameters(short id,
            bool validateExists = true)
        {
            var emailParameters = Repository.Get(id);
            if (emailParameters == null
                && validateExists)
            {
                throw new ExceptionFriendlyMessage(Validator.Messages.EntityNotFound(EmailParametersMetadata.LABEL, id));
            }
            return emailParameters;
        }

        private MailMessage CreateMailMessage(SendWithParametersParameters parameters)
        {
            Validator.ValidateRequiredArgumentAndThrow(parameters, nameof(parameters));

            var mailMessage = new MailMessage();
            mailMessage.SubjectEncoding = mailMessage.BodyEncoding = Encoding.UTF8;

            if (!string.IsNullOrWhiteSpace(parameters.To))
            {
                mailMessage.To.Add(parameters.To);
            }

            if (!string.IsNullOrWhiteSpace(parameters.Parameters.CC))
            {
                mailMessage.Bcc.Add(parameters.Parameters.CC);
            }

            if (!string.IsNullOrWhiteSpace(parameters.BCC))
            {
                mailMessage.Bcc.Add(parameters.BCC);
            }

            mailMessage.Subject = parameters.Parameters.Subject;
            mailMessage.Body = parameters.Parameters.Content;

            if (parameters.BodyVariables != null)
            {
                foreach (var variable in parameters.BodyVariables)
                {
                    mailMessage.Body = mailMessage.Body.Replace("{{" + variable.Key + "}}", variable.Value);
                }
            }

            mailMessage.IsBodyHtml = true;

            if (parameters.Attachments != null)
            {
                foreach (var att in parameters.Attachments)
                {
                    mailMessage.Attachments.Add(new Attachment(new MemoryStream(att.Content), att.Name));
                }
            }

            return mailMessage;
        }
    }
}
