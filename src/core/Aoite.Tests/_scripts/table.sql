/*[Member]*/
CREATE TABLE [Member]
(
	Id bigint NOT NULL IDENTITY(1,1) PRIMARY KEY
	,Username nvarchar(200) NOT NULL
	,[Password] varchar(100) NOT NULL
	,AdvPassword varchar(100) NOT NULL
	,RoleType int NOT NULL DEFAULT(0)
	,VipLevel int NOT NULL DEFAULT(0)
	,Score bigint NOT NULL DEFAULT(0)
	,[Money] bigint NOT NULL DEFAULT(0)

	,Email nvarchar(200) NOT NULL
	,Phone nvarchar(200) NOT NULL
	,RealName nvarchar(200) NOT NULL
	,Sex int NOT NULL DEFAULT(0)
	,IdCard nvarchar(100) NULL
	,IdCardPic nvarchar(100) NULL
	,IsRealAuth bit NOT NULL DEFAULT(0)
	,RealAuthTime datetime NULL
	,RegisterTime datetime NOT NULL

	,CommHaoNum bigint NOT NULL DEFAULT(0)
	,CommZhongNum bigint NOT NULL DEFAULT(0)
	,CommChaNum bigint NOT NULL DEFAULT(0)
	
	,TaskPublishNum bigint NOT NULL DEFAULT(0)
	,TaskFaildNum bigint NOT NULL DEFAULT(0)
	,TaskSuccessNum bigint NOT NULL DEFAULT(0)
	,TaskFailNum bigint NOT NULL DEFAULT(0)
	,[Status] int NOT NULL DEFAULT(0)
)