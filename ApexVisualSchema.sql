/****** Object:  Table [dbo].[ActivityLog]    Script Date: 9/22/2021 6:21:41 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ActivityLog](
	[Id] [uniqueidentifier] NOT NULL,
	[SessionId] [uniqueidentifier] NULL,
	[Username] [varchar](15) NULL,
	[TimeStamp] [datetime] NULL,
	[ApplicationId] [tinyint] NULL,
	[ActivityId] [int] NULL,
	[PackageVersionMajor] [smallint] NULL,
	[PackageVersionMinor] [smallint] NULL,
	[PackageVersionBuild] [smallint] NULL,
	[PackageVersionRevision] [smallint] NULL,
	[Note] [varchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Lap]    Script Date: 9/22/2021 6:21:41 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Lap](
	[Id] [uniqueidentifier] NOT NULL,
	[FromSession] [bigint] NULL,
	[LapNumber] [tinyint] NULL,
	[Sector1Time] [real] NULL,
	[Sector2Time] [real] NULL,
	[Sector3Time] [real] NULL,
	[EndingFuel] [real] NULL,
	[PercentOnThrottle] [tinyint] NULL,
	[PercentOnBrake] [tinyint] NULL,
	[PercentCoasting] [tinyint] NULL,
	[PercentOnMaxThrottle] [tinyint] NULL,
	[PercentOnMaxBrake] [tinyint] NULL,
	[EndingErs] [real] NULL,
	[GearChanges] [smallint] NULL,
	[EquippedTyreCompound] [tinyint] NULL,
	[EndingTyreWear] [uniqueidentifier] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MessageSubmission]    Script Date: 9/22/2021 6:21:41 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MessageSubmission](
	[Id] [uniqueidentifier] NOT NULL,
	[Username] [varchar](15) NULL,
	[Email] [varchar](64) NULL,
	[MessageType] [tinyint] NULL,
	[CreatedAt] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OriginalCapture]    Script Date: 9/22/2021 6:21:41 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OriginalCapture](
	[SessionId] [bigint] NOT NULL,
	[CapturedAtUtc] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[SessionId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Session]    Script Date: 9/22/2021 6:21:41 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Session](
	[SessionId] [bigint] NOT NULL,
	[Owner] [uniqueidentifier] NULL,
	[Game] [tinyint] NULL,
	[Track] [tinyint] NULL,
	[Mode] [tinyint] NULL,
	[Team] [tinyint] NULL,
	[Driver] [tinyint] NULL,
	[CreatedAtUtc] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[SessionId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TelemetrySnapshot]    Script Date: 9/22/2021 6:21:41 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TelemetrySnapshot](
	[Id] [uniqueidentifier] NOT NULL,
	[SessionTime] [real] NULL,
	[LocationType] [tinyint] NULL,
	[LocationNumber] [tinyint] NULL,
	[PositionX] [real] NULL,
	[PositionY] [real] NULL,
	[PositionZ] [real] NULL,
	[CurrentLapTime] [real] NULL,
	[CarPosition] [tinyint] NULL,
	[LapInvalid] [bit] NULL,
	[SpeedKph] [smallint] NULL,
	[Throttle] [tinyint] NULL,
	[Steer] [smallint] NULL,
	[Brake] [tinyint] NULL,
	[Gear] [tinyint] NULL,
	[DrsActive] [bit] NULL,
	[TyreWearPercent] [uniqueidentifier] NULL,
	[TyreDamagePercent] [uniqueidentifier] NULL,
	[StoredErs] [real] NULL,
	[FromLap] [uniqueidentifier] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserAccount]    Script Date: 9/22/2021 6:21:41 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserAccount](
	[Username] [varchar](15) NULL,
	[Password] [varchar](30) NULL,
	[Email] [varchar](64) NULL,
	[AccountCreatedAt] [datetime] NULL,
	[PhotoBlobId] [uniqueidentifier] NULL,
	[Id] [uniqueidentifier] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[WheelDataArray]    Script Date: 9/22/2021 6:21:41 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WheelDataArray](
	[Id] [uniqueidentifier] NOT NULL,
	[RearLeft] [tinyint] NULL,
	[RearRight] [tinyint] NULL,
	[FrontLeft] [tinyint] NULL,
	[FrontRight] [tinyint] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
