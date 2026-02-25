--====
insert into dbo.Parameters (Name,Value) values
('ErrorsDefaultMessage','An error occurred. Please, try again later.'),
('PortalUrl', 'https://localhost:42000'),
('MailsServer','{"PickupDirectory":"D:\\Dev\\mails\\MyApp","Sender":"no-reply@myapp.ccc","BCC":"bcc@myapp.ccc"}'),
('CloudStorageAccountConnectionString', 'UseDevelopmentStorage=true')
go
--====


--====
insert into dbo.EmailsParameters(Id,Subject,Content) values
(1,'Activate your account','<p>Hello {{FullName}},</p><p>In order to activate your account click <a href="{{ActivateAccountUrl}}">here</a> or navigate to the following link in your browser:<br/>{{ActivateAccountUrl}}</p><p>This link is only valid for 24 hours.</p>'),
(2,'Reset your password','<p>Hello {{FullName}},</p><p>In order to reset your password click <a href="{{ResetPasswordUrl}}">here</a> or navigate to the following link in your browser:<br/>{{ResetPasswordUrl}}</p><p>This link is only valid for 24 hours.</p>')
go
--====


--====
insert into dbo.AspNetRoles (Id,Name) values
('00000000-0000-0000-0000-000000000001','Administrator')
go
--====
