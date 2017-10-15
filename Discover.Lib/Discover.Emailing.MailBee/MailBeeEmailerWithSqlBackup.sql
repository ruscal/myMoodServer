/****** Object:  Table [dbo].[DC_EmailCertificate]    Script Date: 05/22/2012 10:52:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DC_EmailCertificate]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[DC_EmailCertificate](
	[EmailAddress] [nvarchar](50) NOT NULL,
	[CertificateName] [nvarchar](200) NOT NULL
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[DC_EmailAttachment]    Script Date: 05/22/2012 10:52:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DC_EmailAttachment]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[DC_EmailAttachment](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EmailId] [int] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Path] [nvarchar](1000) NOT NULL
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[DC_Email]    Script Date: 05/22/2012 10:52:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DC_Email]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[DC_Email](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[BatchId] [nvarchar](50) NULL,
	[Guid] [nvarchar](255) NULL,
	[To] [nvarchar](4000) NULL,
	[FromEmailAddress] [nvarchar](255) NULL,
	[FromName] [nvarchar](255) NULL,
	[Subject] [nvarchar](255) NULL,
	[Message] [ntext] NULL,
	[IsHtml] [bit] NOT NULL,
	[FailedDate] [datetime] NULL,
	[FailedException] [ntext] NULL,
	[FailedAttempts] [int] NOT NULL,
	[SendDate] [datetime] NOT NULL,
	[SendStatus] [nvarchar](20) NOT NULL,
	[IsSigned] [bit] NOT NULL,
	[IsEncrypted] [bit] NOT NULL,
	[DateCreated] [datetime] NOT NULL,
	[CreatedBy] [nvarchar](50) NOT NULL,
	[LastEdited] [datetime] NOT NULL,
	[LastEditedBy] [nvarchar](50) NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
/****** Object:  StoredProcedure [dbo].[dc_spUpdateEmailAttachment]    Script Date: 05/22/2012 10:52:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[dc_spUpdateEmailAttachment]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'create PROCEDURE [dbo].[dc_spUpdateEmailAttachment]
(
	@Name nvarchar(255),
	@Path nvarchar(1000),
	@AttachmentId int
)
AS
	SET NOCOUNT OFF;
UPDATE    DC_EmailAttachment
SET              Name = @Name, Path = @Path
WHERE     (Id = @AttachmentId)
' 
END
GO
/****** Object:  StoredProcedure [dbo].[dc_spUpdateEmail]    Script Date: 05/22/2012 10:52:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[dc_spUpdateEmail]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'create PROCEDURE [dbo].[dc_spUpdateEmail]
(
	@Guid nvarchar(255),
	@BatchId nvarchar(50),
	@To nvarchar(4000),
	@FromEmailAddress nvarchar(255),
	@FromName nvarchar(255),
	@Subject nvarchar(255),
	@Message ntext,
	@IsHtml bit,
	@FailedDate datetime,
	@FailedException ntext,
	@FailedAttempts int,
	@SendDate datetime,
	@IsSigned bit,
	@IsEncrypted bit,
	@SendStatus nvarchar(20),
	@LastEditedBy nvarchar(50),
	@EmailId int
)
AS
	SET NOCOUNT OFF;
UPDATE    DC_Email
SET              [To] = @To,
	[FromEmailAddress] = @FromEmailAddress,
	FromName = @FromName,
	[Subject] = @Subject, 
	[Message] = @Message, 
	IsHtml = @IsHtml, 
    FailedDate = @FailedDate, 
	FailedException = @FailedException, 
	FailedAttempts = @FailedAttempts, 
	SendDate = @SendDate, 
    SendStatus = @SendStatus, 
	IsSigned = @IsSigned, 
	IsEncrypted = @IsEncrypted, 
	LastEdited = GETDATE(), 
	LastEditedBy = @LastEditedBy
WHERE     (Id = @EmailId)
' 
END
GO
/****** Object:  StoredProcedure [dbo].[dc_spInsertEmailAttachment]    Script Date: 05/22/2012 10:52:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[dc_spInsertEmailAttachment]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[dc_spInsertEmailAttachment]
(
	@EmailId int,
	@Name nvarchar(255),
	@Path nvarchar(1000),
	@Id int out
)
AS
	SET NOCOUNT OFF;
INSERT INTO DC_EmailAttachment
                      (EmailId, Name, Path)
VALUES     (@EmailId,@Name,@Path)

SELECT @Id = SCOPE_IDENTITY()
' 
END
GO
/****** Object:  StoredProcedure [dbo].[dc_spInsertEmail]    Script Date: 05/22/2012 10:52:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[dc_spInsertEmail]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[dc_spInsertEmail]
(
	@Guid nvarchar(255),
	@BatchId nvarchar(50),
	@To nvarchar(4000),
	@FromEmailAddress nvarchar(255),
	@FromName nvarchar(255),
	@Subject nvarchar(255),
	@Message ntext,
	@IsHtml bit,
	@FailedDate datetime,
	@FailedException ntext,
	@FailedAttempts int,
	@SendDate datetime,
	@IsSigned bit,
	@IsEncrypted bit,
	@SendStatus nvarchar(20),
	@CreatedBy nvarchar(50),
	@Id int out
)
AS
	SET NOCOUNT OFF;
INSERT INTO DC_Email
                      (Guid, BatchId, [To], [FromEmailAddress], FromName, [Subject], [Message], IsHtml, FailedDate, FailedException, FailedAttempts, SendDate, 
                      SendStatus, IsSigned, IsEncrypted, DateCreated, CreatedBy, LastEdited, LastEditedBy)
VALUES     (@Guid,@BatchId,@To,@FromEmailAddress,@FromName,@Subject,@Message,@IsHtml,@FailedDate,@FailedException,@FailedAttempts,@SendDate,@SendStatus,
				@IsSigned, @IsEncrypted, GETDATE(),@createdby, GETDATE(),@createdby)


SELECT @Id = SCOPE_IDENTITY()
' 
END
GO
/****** Object:  StoredProcedure [dbo].[dc_spGetEmailsByBatchId]    Script Date: 05/22/2012 10:52:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[dc_spGetEmailsByBatchId]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'create PROCEDURE [dbo].[dc_spGetEmailsByBatchId]
	@BatchId nvarchar(255)
AS
	SET NOCOUNT ON;
select * from DC_Email
where BatchId like @BatchId
order by [To] asc
' 
END
GO
/****** Object:  StoredProcedure [dbo].[dc_spGetEmailByGuid]    Script Date: 05/22/2012 10:52:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[dc_spGetEmailByGuid]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'create PROCEDURE [dbo].[dc_spGetEmailByGuid]
(
	@Guid nvarchar(255)
)
AS
	SET NOCOUNT ON;
select * from DC_Email
where Guid = @Guid
' 
END
GO
/****** Object:  StoredProcedure [dbo].[dc_spGetEmailAttachments]    Script Date: 05/22/2012 10:52:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[dc_spGetEmailAttachments]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'create PROCEDURE [dbo].[dc_spGetEmailAttachments]
(
	@EmailId int
)
AS
	SET NOCOUNT ON;
select * from DC_EmailAttachment 
where EmailId = @EmailId
order by Name
' 
END
GO
/****** Object:  StoredProcedure [dbo].[dc_spGetEmailAttachment]    Script Date: 05/22/2012 10:52:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[dc_spGetEmailAttachment]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'create PROCEDURE [dbo].[dc_spGetEmailAttachment]
(
	@AttachmentId int
)
AS
	SET NOCOUNT ON;
select * from DC_EmailAttachment
where Id = @AttachmentId
' 
END
GO
/****** Object:  StoredProcedure [dbo].[dc_spGetEmailAddressCertificateName]    Script Date: 05/22/2012 10:52:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[dc_spGetEmailAddressCertificateName]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'create PROCEDURE [dbo].[dc_spGetEmailAddressCertificateName]
(
	@EmailAddress nvarchar(50)
)
AS
	SET NOCOUNT ON;
select * from DC_EmailCertificate 
where EmailAddress = @EmailAddress
' 
END
GO
/****** Object:  StoredProcedure [dbo].[dc_spGetEmail]    Script Date: 05/22/2012 10:52:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[dc_spGetEmail]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'create PROCEDURE [dbo].[dc_spGetEmail]
(
	@EmailId int
)
AS
	SET NOCOUNT ON;
select * from DC_Email
where Id = @EmailId
' 
END
GO
/****** Object:  StoredProcedure [dbo].[dc_spGetAllPendingEmails]    Script Date: 05/22/2012 10:52:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[dc_spGetAllPendingEmails]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'create PROCEDURE [dbo].[dc_spGetAllPendingEmails]
AS
	SET NOCOUNT ON;
select * from DC_Email
where SendStatus = ''PENDING''
order by DateCreated asc
' 
END
GO
/****** Object:  StoredProcedure [dbo].[dc_spGetAllFailedEmails]    Script Date: 05/22/2012 10:52:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[dc_spGetAllFailedEmails]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'create PROCEDURE [dbo].[dc_spGetAllFailedEmails]
AS
	SET NOCOUNT ON;
select * from DC_Email
where SendStatus = ''FAILED''
order by DateCreated asc
' 
END
GO
/****** Object:  StoredProcedure [dbo].[dc_spGetAllEmailsFromMinsAgo]    Script Date: 05/22/2012 10:52:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[dc_spGetAllEmailsFromMinsAgo]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'create PROCEDURE [dbo].[dc_spGetAllEmailsFromMinsAgo]
(
	@MinsIntoPast int
)
AS
	SET NOCOUNT ON;
select * from DC_Email
where DateDiff(n, SendDate, getdate()) <= @MinsIntoPast
order by DateCreated desc
' 
END
GO
/****** Object:  StoredProcedure [dbo].[dc_spGetAllEmails]    Script Date: 05/22/2012 10:52:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[dc_spGetAllEmails]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'create PROCEDURE [dbo].[dc_spGetAllEmails]
AS
	SET NOCOUNT ON;
select * from DC_Email
order by DateCreated desc
' 
END
GO
/****** Object:  StoredProcedure [dbo].[dc_spFindEmails]    Script Date: 05/22/2012 10:52:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[dc_spFindEmails]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'create PROCEDURE [dbo].[dc_spFindEmails]
(
	@FromDate datetime,
	@ToDate datetime
)
AS
	select * from DC_Email
	where DateCreated >= @FromDate
	and DateCreated < @ToDate
	Order by DateCreated
' 
END
GO
/****** Object:  StoredProcedure [dbo].[dc_spDeleteEmailAttachment]    Script Date: 05/22/2012 10:52:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[dc_spDeleteEmailAttachment]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'create PROCEDURE [dbo].[dc_spDeleteEmailAttachment]
(
	@AttachmentId int
)
AS
	SET NOCOUNT OFF;
DELETE from DC_EmailAttachment
where Id = @AttachmentId
' 
END
GO
/****** Object:  StoredProcedure [dbo].[dc_spDeleteEmail]    Script Date: 05/22/2012 10:52:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[dc_spDeleteEmail]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'create PROCEDURE [dbo].[dc_spDeleteEmail]
(
	@EmailId int
)
AS
	SET NOCOUNT OFF;
DELETE from
DC_Email
where Id = @EmailId
' 
END
GO
