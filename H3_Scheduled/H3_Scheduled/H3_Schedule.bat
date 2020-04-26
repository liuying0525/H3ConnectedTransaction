SET user=System
SET path=%~dp0

C:

CD C:\Windows\System32\

SET name=H3_Scheduled
SCHTASKS /CREATE /TN "%name%" /TR "%path%%name%.exe" /RU "%user%" /SC daily /MO 1 /SD 2019-01-01 /ST 03:00:00 /F
