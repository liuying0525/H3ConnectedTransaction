create table C_FlowSetting
(
       InstanceID char(36) not null,
       SchemaCode nvarchar2(200) not null,
       SettingName nvarchar2(200) not null,
       SettingValue nvarchar2(2000)
)
