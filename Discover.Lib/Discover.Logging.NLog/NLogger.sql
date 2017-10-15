/****** Object:  Table [dbo].[LogEntries]    Script Date: 05/22/2012 10:53:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[LogEntries]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[LogEntries](
	[Id] [int] NOT NULL,
	[TimeStamp] [datetime] NULL,
	[Message] [nvarchar](max) NULL,
	[Level] [nvarchar](10) NULL,
	[Logger] [nvarchar](128) NULL,
	[User] [nvarchar](50) NULL,
	[SessionId] [nvarchar](50) NULL
) ON [PRIMARY]
END
GO
/****** Object:  StoredProcedure [dbo].[spFindLogs]    Script Date: 05/22/2012 10:55:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[spFindLogs]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[spFindLogs]
(
	@FromDate datetime,
	@ToDate datetime,
	@LevelFilter nvarchar(10),
	@LoggerFilter nvarchar(128),
	@MessageFilter nvarchar(200),
	@UserFilter nvarchar(50),
	@SessionFilter nvarchar(50)
)
AS
	SET NOCOUNT ON;
SELECT        *
FROM            LogEntries
where TimeStamp >= @FromDate
and TimeStamp < @ToDate
and (@LevelFilter = '''' or Level like ''%'' + @LevelFilter + ''%'')
and (@LoggerFilter = '''' or Logger like ''%'' + @LoggerFilter + ''%'')
and (@UserFilter = '''' or [User] like ''%'' + @UserFilter + ''%'')
and (@SessionFilter = '''' or SessionId like ''%'' + @SessionFilter + ''%'')
and (@MessageFilter = '''' or Message like ''%'' + @MessageFilter + ''%'')
Order by TimeStamp
' 
END
GO
