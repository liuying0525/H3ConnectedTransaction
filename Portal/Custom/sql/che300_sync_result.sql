--创建日志表，记录che300同步情况
create table Che300_SyncResult
(
      current_version nvarchar2(50) not null,
      latest_version nvarchar2(50) not null,
      sync_date date not null,
      sync_user_name nvarchar2(50) not null,
      sync_user_id nvarchar2(50) not null,
      sync_result clob
)
