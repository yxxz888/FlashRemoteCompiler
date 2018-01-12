@echo off

call "init.bat"

tf get "%1" /r

if %errorlevel% GTR 0 goto err2

echo 获取文件成功
goto :eof

:err2
echo 获取失败,文件已打开或已在本地强制修改过
goto :eof
