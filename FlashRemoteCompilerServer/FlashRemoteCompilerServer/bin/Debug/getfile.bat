@echo off

call "init.bat"

tf get "%1" /r

if %errorlevel% GTR 0 goto err2

echo ��ȡ�ļ��ɹ�
goto :eof

:err2
echo ��ȡʧ��,�ļ��Ѵ򿪻����ڱ���ǿ���޸Ĺ�
goto :eof
