SET PROTOC=protoc.exe
%PROTOC% -I=./ --csharp_out=./ Authentication.proto
%PROTOC% -I=./ --csharp_out=./ GameMulti.proto
%PROTOC% -I=./ --csharp_out=./ GameSingle.proto
%PROTOC% -I=./ --csharp_out=./ Result.proto
%PROTOC% -I=./ --csharp_out=./ User.proto

timeout /t 30