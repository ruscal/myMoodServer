
/****** Object:  Table [dbo].[ML_Phrase]    Script Date: 05/31/2012 12:41:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ML_Phrase]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ML_Phrase](
	[id] [int] NOT NULL,
	[defaultPhrase] [nvarchar](max) NULL,
	[description] [nvarchar](200) NULL,
	[source] [nvarchar](100) NULL,
	[dateCreated] [datetime] NOT NULL,
	[createdBy] [nvarchar](50) NULL,
	[dateLastEdited] [datetime] NOT NULL,
	[lastEditedBy] [nvarchar](50) NULL,
 CONSTRAINT [PK_ML_Phrase] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[ML_Language]    Script Date: 05/31/2012 12:41:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ML_Language]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ML_Language](
	[code] [nvarchar](5) NOT NULL,
	[name] [nvarchar](50) NULL,
	[displayIndex] [int] NOT NULL,
	[enabled] [bit] NOT NULL,
 CONSTRAINT [PK_ML_Language] PRIMARY KEY CLUSTERED 
(
	[code] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[ML_Region]    Script Date: 05/31/2012 12:41:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ML_Region]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ML_Region](
	[Code] [nvarchar](50) NOT NULL,
	[DateDiff] [int] NOT NULL,
	[DateFormat] [nvarchar](50) NOT NULL,
	[DefaultLanguage] [nvarchar](5) NOT NULL,
	[TimeCode] [nvarchar](50) NULL,
 CONSTRAINT [PK_ML_Region] PRIMARY KEY CLUSTERED 
(
	[Code] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[ML_Translation]    Script Date: 05/31/2012 12:41:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ML_Translation]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ML_Translation](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[phraseId] [int] NOT NULL,
	[languageCode] [nvarchar](5) NOT NULL,
	[translationText] [nvarchar](max) NULL,
	[status] [int] NOT NULL,
	[dateCreated] [datetime] NOT NULL,
	[createdBy] [nvarchar](50) NULL,
	[dateLastEdited] [datetime] NOT NULL,
	[lastEditedBy] [nvarchar](50) NULL,
 CONSTRAINT [PK_ML_Translation] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[ML_PhraseReference]    Script Date: 05/31/2012 12:41:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ML_PhraseReference]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ML_PhraseReference](
	[id] [int] NOT NULL,
	[phraseId] [int] NOT NULL,
	[name] [nvarchar](100) NULL,
 CONSTRAINT [PK_ML_Phrase_Reference] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[ML_Culture]    Script Date: 05/31/2012 12:41:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ML_Culture]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ML_Culture](
	[code] [nvarchar](10) NOT NULL,
	[languageCode] [nvarchar](5) NOT NULL,
 CONSTRAINT [PK_ML_Culture] PRIMARY KEY CLUSTERED 
(
	[code] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Default [DF_ML_Language_display_index]    Script Date: 05/31/2012 12:41:09 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_ML_Language_display_index]') AND parent_object_id = OBJECT_ID(N'[dbo].[ML_Language]'))
Begin
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_ML_Language_display_index]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[ML_Language] ADD  CONSTRAINT [DF_ML_Language_display_index]  DEFAULT ((0)) FOR [displayIndex]
END


End
GO
/****** Object:  Default [DF_ML_Language_enabled]    Script Date: 05/31/2012 12:41:09 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_ML_Language_enabled]') AND parent_object_id = OBJECT_ID(N'[dbo].[ML_Language]'))
Begin
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_ML_Language_enabled]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[ML_Language] ADD  CONSTRAINT [DF_ML_Language_enabled]  DEFAULT ((0)) FOR [enabled]
END


End
GO
/****** Object:  Default [DF_ML_Phrase_date_created]    Script Date: 05/31/2012 12:41:09 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_ML_Phrase_date_created]') AND parent_object_id = OBJECT_ID(N'[dbo].[ML_Phrase]'))
Begin
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_ML_Phrase_date_created]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[ML_Phrase] ADD  CONSTRAINT [DF_ML_Phrase_date_created]  DEFAULT (getdate()) FOR [dateCreated]
END


End
GO
/****** Object:  Default [DF_ML_Phrase_date_last_edited]    Script Date: 05/31/2012 12:41:09 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_ML_Phrase_date_last_edited]') AND parent_object_id = OBJECT_ID(N'[dbo].[ML_Phrase]'))
Begin
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_ML_Phrase_date_last_edited]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[ML_Phrase] ADD  CONSTRAINT [DF_ML_Phrase_date_last_edited]  DEFAULT (getdate()) FOR [dateLastEdited]
END


End
GO
/****** Object:  Default [DF_Region_DateDiff]    Script Date: 05/31/2012 12:41:09 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_Region_DateDiff]') AND parent_object_id = OBJECT_ID(N'[dbo].[ML_Region]'))
Begin
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_Region_DateDiff]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[ML_Region] ADD  CONSTRAINT [DF_Region_DateDiff]  DEFAULT ((0)) FOR [DateDiff]
END


End
GO
/****** Object:  Default [DF_Region_DateFormat]    Script Date: 05/31/2012 12:41:09 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_Region_DateFormat]') AND parent_object_id = OBJECT_ID(N'[dbo].[ML_Region]'))
Begin
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_Region_DateFormat]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[ML_Region] ADD  CONSTRAINT [DF_Region_DateFormat]  DEFAULT (N'dd MMMM yyyy HH:mm') FOR [DateFormat]
END


End
GO
/****** Object:  Default [DF_ML_Region_DefaultLanguage]    Script Date: 05/31/2012 12:41:09 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_ML_Region_DefaultLanguage]') AND parent_object_id = OBJECT_ID(N'[dbo].[ML_Region]'))
Begin
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_ML_Region_DefaultLanguage]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[ML_Region] ADD  CONSTRAINT [DF_ML_Region_DefaultLanguage]  DEFAULT (N'en') FOR [DefaultLanguage]
END


End
GO
/****** Object:  Default [DF_ML_Translation_status]    Script Date: 05/31/2012 12:41:09 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_ML_Translation_status]') AND parent_object_id = OBJECT_ID(N'[dbo].[ML_Translation]'))
Begin
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_ML_Translation_status]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[ML_Translation] ADD  CONSTRAINT [DF_ML_Translation_status]  DEFAULT ((1)) FOR [status]
END


End
GO
/****** Object:  Default [DF_ML_Translation_date_created]    Script Date: 05/31/2012 12:41:09 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_ML_Translation_date_created]') AND parent_object_id = OBJECT_ID(N'[dbo].[ML_Translation]'))
Begin
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_ML_Translation_date_created]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[ML_Translation] ADD  CONSTRAINT [DF_ML_Translation_date_created]  DEFAULT (getdate()) FOR [dateCreated]
END


End
GO
/****** Object:  Default [DF_ML_Translation_date_last_edited]    Script Date: 05/31/2012 12:41:09 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_ML_Translation_date_last_edited]') AND parent_object_id = OBJECT_ID(N'[dbo].[ML_Translation]'))
Begin
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_ML_Translation_date_last_edited]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[ML_Translation] ADD  CONSTRAINT [DF_ML_Translation_date_last_edited]  DEFAULT (getdate()) FOR [dateLastEdited]
END


End
GO
/****** Object:  ForeignKey [FK_ML_Culture_ML_Language]    Script Date: 05/31/2012 12:41:09 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ML_Culture_ML_Language]') AND parent_object_id = OBJECT_ID(N'[dbo].[ML_Culture]'))
ALTER TABLE [dbo].[ML_Culture]  WITH CHECK ADD  CONSTRAINT [FK_ML_Culture_ML_Language] FOREIGN KEY([languageCode])
REFERENCES [dbo].[ML_Language] ([code])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ML_Culture_ML_Language]') AND parent_object_id = OBJECT_ID(N'[dbo].[ML_Culture]'))
ALTER TABLE [dbo].[ML_Culture] CHECK CONSTRAINT [FK_ML_Culture_ML_Language]
GO
/****** Object:  ForeignKey [FK_ML_Phrase_Reference_ML_Phrase]    Script Date: 05/31/2012 12:41:09 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ML_Phrase_Reference_ML_Phrase]') AND parent_object_id = OBJECT_ID(N'[dbo].[ML_PhraseReference]'))
ALTER TABLE [dbo].[ML_PhraseReference]  WITH CHECK ADD  CONSTRAINT [FK_ML_Phrase_Reference_ML_Phrase] FOREIGN KEY([phraseId])
REFERENCES [dbo].[ML_Phrase] ([id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ML_Phrase_Reference_ML_Phrase]') AND parent_object_id = OBJECT_ID(N'[dbo].[ML_PhraseReference]'))
ALTER TABLE [dbo].[ML_PhraseReference] CHECK CONSTRAINT [FK_ML_Phrase_Reference_ML_Phrase]
GO
/****** Object:  ForeignKey [FK_ML_Translation_ML_Language]    Script Date: 05/31/2012 12:41:09 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ML_Translation_ML_Language]') AND parent_object_id = OBJECT_ID(N'[dbo].[ML_Translation]'))
ALTER TABLE [dbo].[ML_Translation]  WITH CHECK ADD  CONSTRAINT [FK_ML_Translation_ML_Language] FOREIGN KEY([languageCode])
REFERENCES [dbo].[ML_Language] ([code])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ML_Translation_ML_Language]') AND parent_object_id = OBJECT_ID(N'[dbo].[ML_Translation]'))
ALTER TABLE [dbo].[ML_Translation] CHECK CONSTRAINT [FK_ML_Translation_ML_Language]
GO
/****** Object:  ForeignKey [FK_ML_Translation_ML_Phrase1]    Script Date: 05/31/2012 12:41:09 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ML_Translation_ML_Phrase1]') AND parent_object_id = OBJECT_ID(N'[dbo].[ML_Translation]'))
ALTER TABLE [dbo].[ML_Translation]  WITH CHECK ADD  CONSTRAINT [FK_ML_Translation_ML_Phrase1] FOREIGN KEY([phraseId])
REFERENCES [dbo].[ML_Phrase] ([id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ML_Translation_ML_Phrase1]') AND parent_object_id = OBJECT_ID(N'[dbo].[ML_Translation]'))
ALTER TABLE [dbo].[ML_Translation] CHECK CONSTRAINT [FK_ML_Translation_ML_Phrase1]
GO



/****** Object:  StoredProcedure [dbo].[ML_DeleteCulture]    Script Date: 05/31/2012 12:43:39 ******/
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[ML_DeleteCulture]') AND type = 'P')
BEGIN
DROP PROCEDURE [dbo].[ML_DeleteCulture]
END
GO
/****** Object:  StoredProcedure [dbo].[ML_DeletePhraseReference]    Script Date: 05/31/2012 12:43:39 ******/
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[ML_DeletePhraseReference]') AND type = 'P')
BEGIN
DROP PROCEDURE [dbo].[ML_DeletePhraseReference]
END
GO
/****** Object:  StoredProcedure [dbo].[ML_DeleteTranslation]    Script Date: 05/31/2012 12:43:39 ******/
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[ML_DeleteTranslation]') AND type = 'P')
BEGIN
DROP PROCEDURE [dbo].[ML_DeleteTranslation]
END
GO
/****** Object:  StoredProcedure [dbo].[ML_GetAllTranslation]    Script Date: 05/31/2012 12:43:39 ******/
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[ML_GetAllTranslation]') AND type = 'P')
BEGIN
DROP PROCEDURE [dbo].[ML_GetAllTranslation]
END
GO
/****** Object:  StoredProcedure [dbo].[ML_GetAllTranslations]    Script Date: 05/31/2012 12:43:39 ******/
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[ML_GetAllTranslations]') AND type = 'P')
BEGIN
DROP PROCEDURE [dbo].[ML_GetAllTranslations]
END
GO
/****** Object:  StoredProcedure [dbo].[ML_GetAllTranslationsByLanguage]    Script Date: 05/31/2012 12:43:39 ******/
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[ML_GetAllTranslationsByLanguage]') AND type = 'P')
BEGIN
DROP PROCEDURE [dbo].[ML_GetAllTranslationsByLanguage]
END
GO
/****** Object:  StoredProcedure [dbo].[ML_GetCulture]    Script Date: 05/31/2012 12:43:39 ******/
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[ML_GetCulture]') AND type = 'P')
BEGIN
DROP PROCEDURE [dbo].[ML_GetCulture]
END
GO
/****** Object:  StoredProcedure [dbo].[ML_GetCulturesByLanguage]    Script Date: 05/31/2012 12:43:39 ******/
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[ML_GetCulturesByLanguage]') AND type = 'P')
BEGIN
DROP PROCEDURE [dbo].[ML_GetCulturesByLanguage]
END
GO
/****** Object:  StoredProcedure [dbo].[ML_InsertTranslation]    Script Date: 05/31/2012 12:43:39 ******/
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[ML_InsertTranslation]') AND type = 'P')
BEGIN
DROP PROCEDURE [dbo].[ML_InsertTranslation]
END
GO
/****** Object:  StoredProcedure [dbo].[ML_GetAllPhraseReferences]    Script Date: 05/31/2012 12:43:39 ******/
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[ML_GetAllPhraseReferences]') AND type = 'P')
BEGIN
DROP PROCEDURE [dbo].[ML_GetAllPhraseReferences]
END
GO
/****** Object:  StoredProcedure [dbo].[ML_GetPhraseReference]    Script Date: 05/31/2012 12:43:39 ******/
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[ML_GetPhraseReference]') AND type = 'P')
BEGIN
DROP PROCEDURE [dbo].[ML_GetPhraseReference]
END
GO
/****** Object:  StoredProcedure [dbo].[ML_GetTranslation]    Script Date: 05/31/2012 12:43:39 ******/
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[ML_GetTranslation]') AND type = 'P')
BEGIN
DROP PROCEDURE [dbo].[ML_GetTranslation]
END
GO
/****** Object:  StoredProcedure [dbo].[ML_GetTranslationByPhraseId]    Script Date: 05/31/2012 12:43:39 ******/
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[ML_GetTranslationByPhraseId]') AND type = 'P')
BEGIN
DROP PROCEDURE [dbo].[ML_GetTranslationByPhraseId]
END
GO
/****** Object:  StoredProcedure [dbo].[ML_GetTranslationByReferenceId]    Script Date: 05/31/2012 12:43:39 ******/
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[ML_GetTranslationByReferenceId]') AND type = 'P')
BEGIN
DROP PROCEDURE [dbo].[ML_GetTranslationByReferenceId]
END
GO
/****** Object:  StoredProcedure [dbo].[ML_GetTranslationsByLanguage]    Script Date: 05/31/2012 12:43:39 ******/
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[ML_GetTranslationsByLanguage]') AND type = 'P')
BEGIN
DROP PROCEDURE [dbo].[ML_GetTranslationsByLanguage]
END
GO
/****** Object:  StoredProcedure [dbo].[ML_InsertCulture]    Script Date: 05/31/2012 12:43:39 ******/
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[ML_InsertCulture]') AND type = 'P')
BEGIN
DROP PROCEDURE [dbo].[ML_InsertCulture]
END
GO
/****** Object:  StoredProcedure [dbo].[ML_InsertPhraseReference]    Script Date: 05/31/2012 12:43:39 ******/
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[ML_InsertPhraseReference]') AND type = 'P')
BEGIN
DROP PROCEDURE [dbo].[ML_InsertPhraseReference]
END
GO
/****** Object:  StoredProcedure [dbo].[ML_UpdateCulture]    Script Date: 05/31/2012 12:43:39 ******/
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[ML_UpdateCulture]') AND type = 'P')
BEGIN
DROP PROCEDURE [dbo].[ML_UpdateCulture]
END
GO
/****** Object:  StoredProcedure [dbo].[ML_UpdatePhraseReference]    Script Date: 05/31/2012 12:43:39 ******/
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[ML_UpdatePhraseReference]') AND type = 'P')
BEGIN
DROP PROCEDURE [dbo].[ML_UpdatePhraseReference]
END
GO
/****** Object:  StoredProcedure [dbo].[ML_UpdateTranslation]    Script Date: 05/31/2012 12:43:39 ******/
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[ML_UpdateTranslation]') AND type = 'P')
BEGIN
DROP PROCEDURE [dbo].[ML_UpdateTranslation]
END
GO
/****** Object:  StoredProcedure [dbo].[ML_UpdateLanguage]    Script Date: 05/31/2012 12:43:39 ******/
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[ML_UpdateLanguage]') AND type = 'P')
BEGIN
DROP PROCEDURE [dbo].[ML_UpdateLanguage]
END
GO
/****** Object:  StoredProcedure [dbo].[ML_UpdatePhrase]    Script Date: 05/31/2012 12:43:39 ******/
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[ML_UpdatePhrase]') AND type = 'P')
BEGIN
DROP PROCEDURE [dbo].[ML_UpdatePhrase]
END
GO
/****** Object:  StoredProcedure [dbo].[ML_InsertRegion]    Script Date: 05/31/2012 12:43:39 ******/
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[ML_InsertRegion]') AND type = 'P')
BEGIN
DROP PROCEDURE [dbo].[ML_InsertRegion]
END
GO
/****** Object:  StoredProcedure [dbo].[ML_InsertLanguage]    Script Date: 05/31/2012 12:43:39 ******/
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[ML_InsertLanguage]') AND type = 'P')
BEGIN
DROP PROCEDURE [dbo].[ML_InsertLanguage]
END
GO
/****** Object:  StoredProcedure [dbo].[ML_InsertPhrase]    Script Date: 05/31/2012 12:43:39 ******/
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[ML_InsertPhrase]') AND type = 'P')
BEGIN
DROP PROCEDURE [dbo].[ML_InsertPhrase]
END
GO
/****** Object:  StoredProcedure [dbo].[ML_GetRegion]    Script Date: 05/31/2012 12:43:39 ******/
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[ML_GetRegion]') AND type = 'P')
BEGIN
DROP PROCEDURE [dbo].[ML_GetRegion]
END
GO
/****** Object:  StoredProcedure [dbo].[ML_GetAllPhrases]    Script Date: 05/31/2012 12:43:39 ******/
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[ML_GetAllPhrases]') AND type = 'P')
BEGIN
DROP PROCEDURE [dbo].[ML_GetAllPhrases]
END
GO
/****** Object:  StoredProcedure [dbo].[ML_GetAllRegions]    Script Date: 05/31/2012 12:43:39 ******/
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[ML_GetAllRegions]') AND type = 'P')
BEGIN
DROP PROCEDURE [dbo].[ML_GetAllRegions]
END
GO
/****** Object:  StoredProcedure [dbo].[ML_GetLanguage]    Script Date: 05/31/2012 12:43:39 ******/
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[ML_GetLanguage]') AND type = 'P')
BEGIN
DROP PROCEDURE [dbo].[ML_GetLanguage]
END
GO
/****** Object:  StoredProcedure [dbo].[ML_GetLanguageByName]    Script Date: 05/31/2012 12:43:39 ******/
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[ML_GetLanguageByName]') AND type = 'P')
BEGIN
DROP PROCEDURE [dbo].[ML_GetLanguageByName]
END
GO
/****** Object:  StoredProcedure [dbo].[ML_GetPhrase]    Script Date: 05/31/2012 12:43:39 ******/
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[ML_GetPhrase]') AND type = 'P')
BEGIN
DROP PROCEDURE [dbo].[ML_GetPhrase]
END
GO
/****** Object:  StoredProcedure [dbo].[ML_GetPhraseByText]    Script Date: 05/31/2012 12:43:39 ******/
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[ML_GetPhraseByText]') AND type = 'P')
BEGIN
DROP PROCEDURE [dbo].[ML_GetPhraseByText]
END
GO
/****** Object:  StoredProcedure [dbo].[ML_GetAllLanguages]    Script Date: 05/31/2012 12:43:39 ******/
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[ML_GetAllLanguages]') AND type = 'P')
BEGIN
DROP PROCEDURE [dbo].[ML_GetAllLanguages]
END
GO
/****** Object:  StoredProcedure [dbo].[ML_DeleteLanguage]    Script Date: 05/31/2012 12:43:39 ******/
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[ML_DeleteLanguage]') AND type = 'P')
BEGIN
DROP PROCEDURE [dbo].[ML_DeleteLanguage]
END
GO
/****** Object:  StoredProcedure [dbo].[ML_DeletePhrase]    Script Date: 05/31/2012 12:43:39 ******/
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[ML_DeletePhrase]') AND type = 'P')
BEGIN
DROP PROCEDURE [dbo].[ML_DeletePhrase]
END
GO
/****** Object:  StoredProcedure [dbo].[ML_DeletePhrase]    Script Date: 05/31/2012 12:43:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[ML_DeletePhrase]
(
	@id int
)
AS
	SET NOCOUNT OFF;
DELETE FROM [ML_Phrase] WHERE [id] = @id
GO
/****** Object:  StoredProcedure [dbo].[ML_DeleteLanguage]    Script Date: 05/31/2012 12:43:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[ML_DeleteLanguage]
(
	@code nvarchar(5)
)
AS
	SET NOCOUNT OFF;
DELETE FROM [ML_Language] WHERE [code] = @code
GO
/****** Object:  StoredProcedure [dbo].[ML_GetAllLanguages]    Script Date: 05/31/2012 12:43:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[ML_GetAllLanguages]
AS
	SET NOCOUNT ON;
select * from ML_Language
order by DisplayIndex, Name
GO
/****** Object:  StoredProcedure [dbo].[ML_GetPhraseByText]    Script Date: 05/31/2012 12:43:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[ML_GetPhraseByText]
(
	@Text nvarchar(max)
)
AS
	SET NOCOUNT ON;
select * from ML_Phrase
where defaultPhrase = @Text
GO
/****** Object:  StoredProcedure [dbo].[ML_GetPhrase]    Script Date: 05/31/2012 12:43:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[ML_GetPhrase]
(
	@Id int
)
AS
	SET NOCOUNT ON;
select * from ML_Phrase
where Id = @Id
GO
/****** Object:  StoredProcedure [dbo].[ML_GetLanguageByName]    Script Date: 05/31/2012 12:43:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[ML_GetLanguageByName]
(
	@LanguageName nvarchar(50)
)
AS
	SET NOCOUNT ON;
select * from ML_Language
where Name = @LanguageName
GO
/****** Object:  StoredProcedure [dbo].[ML_GetLanguage]    Script Date: 05/31/2012 12:43:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[ML_GetLanguage]
(
	@Code nvarchar(5)
)
AS
	SET NOCOUNT ON;
select * from ML_Language
where code = @Code
GO
/****** Object:  StoredProcedure [dbo].[ML_GetAllRegions]    Script Date: 05/31/2012 12:43:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[ML_GetAllRegions]
AS
	SET NOCOUNT ON;
Select * from
dbo.ML_Region
order by Code
GO
/****** Object:  StoredProcedure [dbo].[ML_GetAllPhrases]    Script Date: 05/31/2012 12:43:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create PROCEDURE [dbo].[ML_GetAllPhrases]

AS
	SET NOCOUNT ON;
select * from ML_Phrase
Order by id
GO
/****** Object:  StoredProcedure [dbo].[ML_GetRegion]    Script Date: 05/31/2012 12:43:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[ML_GetRegion]
(
	@Code nvarchar(50)
)
AS
	SET NOCOUNT ON;
SELECT       *
FROM            ML_Region
where Code = @Code
GO
/****** Object:  StoredProcedure [dbo].[ML_InsertPhrase]    Script Date: 05/31/2012 12:43:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[ML_InsertPhrase]
(
	@defaultPhrase nvarchar(MAX),
	@description nvarchar(200),
	@source nvarchar(100),
	@dateCreated datetime,
	@createdBy nvarchar(50),
	@dateLastEdited datetime,
	@lastEditedBy nvarchar(50),
	@id int output
)
AS
	SET NOCOUNT OFF;
INSERT INTO [ML_Phrase] ([defaultPhrase], [description], [source], [dateCreated], [createdBy], [dateLastEdited], [lastEditedBy]) VALUES (@defaultPhrase, @description, @source, @dateCreated, @createdBy, @dateLastEdited, @lastEditedBy);
	
SELECT @Id = SCOPE_IDENTITY()
GO
/****** Object:  StoredProcedure [dbo].[ML_InsertLanguage]    Script Date: 05/31/2012 12:43:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[ML_InsertLanguage]
(
	@code nvarchar(5),
	@name nvarchar(50),
	@displayIndex int,
	@enabled bit
)
AS
	SET NOCOUNT OFF;

if not exists(select * from [ML_Language] where code = @code)	
	INSERT INTO [ML_Language] ([code], [name], [displayIndex], [enabled]) VALUES (@code, @name, @displayIndex, @enabled);
GO
/****** Object:  StoredProcedure [dbo].[ML_InsertRegion]    Script Date: 05/31/2012 12:43:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[ML_InsertRegion]
(
	@Code nvarchar(50),
	@DateDiff int,
	@DateFormat nvarchar(50),
	@DefaultLanguage nvarchar(5),
	@TimeCode	nvarchar(50)
)
AS
	SET NOCOUNT OFF;
INSERT INTO [ML_Region] ([Code], [DateDiff], [DateFormat], [DefaultLanguage], [TimeCode]) 
VALUES (@Code, @DateDiff, @DateFormat, @DefaultLanguage, @TimeCode)
GO
/****** Object:  StoredProcedure [dbo].[ML_UpdatePhrase]    Script Date: 05/31/2012 12:43:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[ML_UpdatePhrase]
(
	@defaultPhrase nvarchar(MAX),
	@description nvarchar(200),
	@source nvarchar(100),
	@dateCreated datetime,
	@createdBy nvarchar(50),
	@dateLastEdited datetime,
	@lastEditedBy nvarchar(50),
	@Original_id int,
	@IsNull_description Int,
	@Original_description nvarchar(200),
	@IsNull_source Int,
	@Original_source nvarchar(100),
	@Original_dateCreated datetime,
	@IsNull_createdBy Int,
	@Original_createdBy nvarchar(50),
	@Original_dateLastEdited datetime,
	@IsNull_lastEditedBy Int,
	@Original_lastEditedBy nvarchar(50),
	@id int
)
AS
	SET NOCOUNT OFF;
UPDATE [ML_Phrase] SET [defaultPhrase] = @defaultPhrase, [description] = @description, [source] = @source, [dateCreated] = @dateCreated, [createdBy] = @createdBy, [dateLastEdited] = @dateLastEdited, [lastEditedBy] = @lastEditedBy WHERE (([id] = @Original_id) AND ((@IsNull_description = 1 AND [description] IS NULL) OR ([description] = @Original_description)) AND ((@IsNull_source = 1 AND [source] IS NULL) OR ([source] = @Original_source)) AND ([dateCreated] = @Original_dateCreated) AND ((@IsNull_createdBy = 1 AND [createdBy] IS NULL) OR ([createdBy] = @Original_createdBy)) AND ([dateLastEdited] = @Original_dateLastEdited) AND ((@IsNull_lastEditedBy = 1 AND [lastEditedBy] IS NULL) OR ([lastEditedBy] = @Original_lastEditedBy)));
	
SELECT id, defaultPhrase, description, source, dateCreated, createdBy, dateLastEdited, lastEditedBy FROM ML_Phrase WHERE (id = @id)
GO
/****** Object:  StoredProcedure [dbo].[ML_UpdateLanguage]    Script Date: 05/31/2012 12:43:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[ML_UpdateLanguage]
(
	@code nvarchar(5),
	@name nvarchar(50),
	@displayIndex int,
	@enabled bit,
	@Original_code nvarchar(5),
	@IsNull_name Int,
	@Original_name nvarchar(50),
	@Original_displayIndex int,
	@Original_enabled bit
)
AS
	SET NOCOUNT OFF;
UPDATE [ML_Language] SET [code] = @code, [name] = @name, [displayIndex] = @displayIndex, [enabled] = @enabled WHERE (([code] = @Original_code) AND ((@IsNull_name = 1 AND [name] IS NULL) OR ([name] = @Original_name)) AND ([displayIndex] = @Original_displayIndex) AND ([enabled] = @Original_enabled));
	
SELECT code, name, displayIndex, enabled FROM ML_Language WHERE (code = @code)
GO
/****** Object:  StoredProcedure [dbo].[ML_UpdateTranslation]    Script Date: 05/31/2012 12:43:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[ML_UpdateTranslation]
(
	@phraseId int,
	@languageCode nvarchar(5),
	@translationText nvarchar(MAX),
	@status int,
	@dateCreated datetime,
	@createdBy nvarchar(50),
	@dateLastEdited datetime,
	@lastEditedBy nvarchar(50),
	@Original_id int,
	@Original_phraseId int,
	@Original_languageCode nvarchar(5),
	@Original_status int,
	@Original_dateCreated datetime,
	@IsNull_createdBy Int,
	@Original_createdBy nvarchar(50),
	@Original_dateLastEdited datetime,
	@IsNull_lastEditedBy Int,
	@Original_lastEditedBy nvarchar(50),
	@id int
)
AS
	SET NOCOUNT OFF;
UPDATE [ML_Translation] SET [phraseId] = @phraseId, [languageCode] = @languageCode, [translationText] = @translationText, [status] = @status, [dateCreated] = @dateCreated, [createdBy] = @createdBy, [dateLastEdited] = @dateLastEdited, [lastEditedBy] = @lastEditedBy WHERE (([id] = @Original_id) AND ([phraseId] = @Original_phraseId) AND ([languageCode] = @Original_languageCode) AND ([status] = @Original_status) AND ([dateCreated] = @Original_dateCreated) AND ((@IsNull_createdBy = 1 AND [createdBy] IS NULL) OR ([createdBy] = @Original_createdBy)) AND ([dateLastEdited] = @Original_dateLastEdited) AND ((@IsNull_lastEditedBy = 1 AND [lastEditedBy] IS NULL) OR ([lastEditedBy] = @Original_lastEditedBy)));
	
SELECT id, phraseId, languageCode, translationText, status, dateCreated, createdBy, dateLastEdited, lastEditedBy FROM ML_Translation WHERE (id = @id)
GO
/****** Object:  StoredProcedure [dbo].[ML_UpdatePhraseReference]    Script Date: 05/31/2012 12:43:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[ML_UpdatePhraseReference]
(
	@phraseId int,
	@name nvarchar(100),
	@Original_id int,
	@Original_phraseId int,
	@IsNull_name Int,
	@Original_name nvarchar(100),
	@id int
)
AS
	SET NOCOUNT OFF;
UPDATE [ML_PhraseReference] SET [phraseId] = @phraseId, [name] = @name WHERE (([id] = @Original_id) AND ([phraseId] = @Original_phraseId) AND ((@IsNull_name = 1 AND [name] IS NULL) OR ([name] = @Original_name)));
	
SELECT id, phraseId, name FROM ML_PhraseReference WHERE (id = @id)
GO
/****** Object:  StoredProcedure [dbo].[ML_UpdateCulture]    Script Date: 05/31/2012 12:43:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[ML_UpdateCulture]
(
	@code nvarchar(10),
	@languageCode nvarchar(5),
	@Original_code nvarchar(10),
	@Original_languageCode nvarchar(5)
)
AS
	SET NOCOUNT OFF;
UPDATE [ML_Culture] SET [code] = @code, [languageCode] = @languageCode WHERE (([code] = @Original_code) AND ([languageCode] = @Original_languageCode));
	
SELECT code, languageCode FROM ML_Culture WHERE (code = @code)
GO
/****** Object:  StoredProcedure [dbo].[ML_InsertPhraseReference]    Script Date: 05/31/2012 12:43:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[ML_InsertPhraseReference]
(
	@phraseId int,
	@name nvarchar(100),
	@id int output
)
AS
	SET NOCOUNT OFF;
INSERT INTO [ML_PhraseReference] ([phraseId], [name]) VALUES (@phraseId, @name);
	
SELECT @Id = SCOPE_IDENTITY()
GO
/****** Object:  StoredProcedure [dbo].[ML_InsertCulture]    Script Date: 05/31/2012 12:43:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[ML_InsertCulture]
(
	@code nvarchar(10),
	@languageCode nvarchar(5)
)
AS
	SET NOCOUNT OFF;
	
if not exists(select * from [ML_Culture] where code = @code)	
	INSERT INTO [ML_Culture] ([code], [languageCode]) VALUES (@code, @languageCode);
GO
/****** Object:  StoredProcedure [dbo].[ML_GetTranslationsByLanguage]    Script Date: 05/31/2012 12:43:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create PROCEDURE [dbo].[ML_GetTranslationsByLanguage]
(
	@LanguageCode nvarchar(5)
)
AS
	SET NOCOUNT ON;
select distinct t.* from ML_Translation t
where t.languageCode = @LanguageCode
order by phraseId
GO
/****** Object:  StoredProcedure [dbo].[ML_GetTranslationByReferenceId]    Script Date: 05/31/2012 12:43:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[ML_GetTranslationByReferenceId]
(
	@PhraseReferenceId int,
	@LanguageCode nvarchar(5)
)
AS
	SET NOCOUNT ON;
select distinct t.* from ML_Translation t
inner join ML_PhraseReference r
on r.PhraseId = t.PhraseId
where r.Id = @PhraseReferenceId
and t.languageCode = @LanguageCode
GO
/****** Object:  StoredProcedure [dbo].[ML_GetTranslationByPhraseId]    Script Date: 05/31/2012 12:43:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[ML_GetTranslationByPhraseId]
(
	@PhraseId int,
	@LanguageCode nvarchar(5)
)
AS
	SET NOCOUNT ON;
select distinct t.* from ML_Translation t
where t.PhraseId = @PhraseId
and t.languageCode = @LanguageCode
GO
/****** Object:  StoredProcedure [dbo].[ML_GetTranslation]    Script Date: 05/31/2012 12:43:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[ML_GetTranslation]
(
	@Id int
)
AS
	SET NOCOUNT ON;
select * from ML_Translation
where Id = @Id
GO
/****** Object:  StoredProcedure [dbo].[ML_GetPhraseReference]    Script Date: 05/31/2012 12:43:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[ML_GetPhraseReference]
(
	@Id int
)
AS
	SET NOCOUNT ON;
select * from ML_PhraseReference
where Id =@Id
GO
/****** Object:  StoredProcedure [dbo].[ML_GetAllPhraseReferences]    Script Date: 05/31/2012 12:43:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[ML_GetAllPhraseReferences]

AS
	SET NOCOUNT ON;
select * from ML_PhraseReference
Order by Id
GO
/****** Object:  StoredProcedure [dbo].[ML_InsertTranslation]    Script Date: 05/31/2012 12:43:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[ML_InsertTranslation]
(
	@phraseId int,
	@languageCode nvarchar(5),
	@translationText nvarchar(MAX),
	@status int,
	@dateCreated datetime,
	@createdBy nvarchar(50),
	@dateLastEdited datetime,
	@lastEditedBy nvarchar(50),
	@id int output
)
AS
	SET NOCOUNT OFF;
INSERT INTO [ML_Translation] ([phraseId], [languageCode], [translationText], [status], [dateCreated], [createdBy], [dateLastEdited], [lastEditedBy]) VALUES (@phraseId, @languageCode, @translationText, @status, @dateCreated, @createdBy, @dateLastEdited, @lastEditedBy);
	
SELECT @Id = SCOPE_IDENTITY()
GO
/****** Object:  StoredProcedure [dbo].[ML_GetCulturesByLanguage]    Script Date: 05/31/2012 12:43:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[ML_GetCulturesByLanguage]
(
	@LanguageCode nvarchar(5)
)
AS
	SET NOCOUNT ON;
select * from ML_Culture
where languageCode = @LanguageCode
order by code
GO
/****** Object:  StoredProcedure [dbo].[ML_GetCulture]    Script Date: 05/31/2012 12:43:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[ML_GetCulture]
(
	@Code nvarchar(10)
)
AS
	SET NOCOUNT ON;
select * from ML_Culture
where code = @Code
GO
/****** Object:  StoredProcedure [dbo].[ML_GetAllTranslationsByLanguage]    Script Date: 05/31/2012 12:43:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create PROCEDURE [dbo].[ML_GetAllTranslationsByLanguage]
(
	@LanguageCode nvarchar(5)
)
AS
	SET NOCOUNT ON;
select * from ML_Translation
where LanguageCode = @LanguageCode
order by PhraseID
GO
/****** Object:  StoredProcedure [dbo].[ML_GetAllTranslations]    Script Date: 05/31/2012 12:43:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[ML_GetAllTranslations]

AS
	SET NOCOUNT ON;
select * from ML_Translation
order by PhraseID
GO
/****** Object:  StoredProcedure [dbo].[ML_GetAllTranslation]    Script Date: 05/31/2012 12:43:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create PROCEDURE [dbo].[ML_GetAllTranslation]
(
	@LanguageCode nvarchar(5)
)
AS
	SET NOCOUNT ON;
select * from ML_Translation
where LanguageCode = @LanguageCode
order by PhraseID
GO
/****** Object:  StoredProcedure [dbo].[ML_DeleteTranslation]    Script Date: 05/31/2012 12:43:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[ML_DeleteTranslation]
(
	@id int
)
AS
	SET NOCOUNT OFF;
DELETE FROM [ML_Translation] WHERE [id] = @id
GO
/****** Object:  StoredProcedure [dbo].[ML_DeletePhraseReference]    Script Date: 05/31/2012 12:43:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[ML_DeletePhraseReference]
(
	@id int
)
AS
	SET NOCOUNT OFF;
DELETE FROM [ML_PhraseReference] WHERE [id] = @id
GO
/****** Object:  StoredProcedure [dbo].[ML_DeleteCulture]    Script Date: 05/31/2012 12:43:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[ML_DeleteCulture]
(
	@code nvarchar(10)
)
AS
	SET NOCOUNT OFF;
DELETE FROM [ML_Culture] WHERE [code] = @code
GO
