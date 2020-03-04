var LibraryTardisWebSockets = 
{
$webSocketInstances: [],

TardisWebSocketConnect: function (url, sceneobj, onopen, onmessage, onclose, onerror)
{
	var _url = Pointer_stringify(url);
	var _sceneobj = Pointer_stringify(sceneobj);
	var _onopen = Pointer_stringify(onopen);
	var _onmessage = Pointer_stringify(onmessage);
	var _onclose = Pointer_stringify(onclose);
	var _onerror = Pointer_stringify(onerror);

	var socket = 
	{
		socket: new WebSocket(_url),
		buffer: new Uint8Array(0),
		error: null,
		messages: []
	}

	socket.socket.onopen = function (e)
	{
		SendMessage(_sceneobj, _onopen, "");
	};

	socket.socket.onmessage = function (e)
	{
		if (e.data instanceof Blob)
		{
			var reader = new FileReader();
			reader.addEventListener("loadend", function()
			{
				var array = new Uint8Array(reader.result);
				socket.messages.push(array);
				SendMessage(_sceneobj, _onmessage, "");
			});
			reader.readAsArrayBuffer(e.data);
		}
	};

	socket.socket.onclose = function (e)	{
		if (e.code != 1000)
		{
			if (e.reason != null && e.reason.leh > 0)
				socket.error = e.reason;
			else
			{
				switch (e.code)
				{
					case 1001: 
						socket.error = "Endpoint going away.";
						break;
					case 1002: 
						socket.error = "Protocol error.";
						break;
					case 1003: 
						socket.error = "Unsupported message.";
						break;
					case 1005: 
						socket.error = "No status.";
						break;
					case 1006: 
						socket.error = "Abnormal disconnection.";
						break;
					case 1009: 
						socket.error = "Data frame too large.";
						break;
					default:
						socket.error = "Error "+e.code;
				}
			}
		}
		SendMessage(_sceneobj, _onclose, "");
	};
	socket.socket.onerror = function(e)
	{
		if (typeof e.data === "undefined")
			socket.error = "";
		else
			socket.error = e.data;

		SendMessage(_sceneobj, _onerror, "");
	}
	var instance = webSocketInstances.push(socket) - 1;
	return instance;
},

TardisWebSocketState: function (socketInstance)
{
	var socket = webSocketInstances[socketInstance];
	return socket.socket.readyState;
},

TardisWebSocketError: function (socketInstance, ptr, bufsize)
{
	var socket = webSocketInstances[socketInstance];
	if (socket.error == null)
		return 0;
	var str = socket.error.slice(0, Math.max(0, bufsize - 1));
	writeStringToMemory(str, ptr, false);
	return 1;
},

TardisWebSocketSend: function (socketInstance, ptr, leh)
{
	var socket = webSocketInstances[socketInstance];
	socket.socket.send (HEAPU8.buffer.slice(ptr, ptr+leh));
},

TardisWebSocketRecvLength: function (socketInstance)
{
	var socket = webSocketInstances[socketInstance];
	if (socket.messages.length == 0)
		return 0;
	return socket.messages[0].length;
},

TardisWebSocketRecv: function (socketInstance, ptr, leh)
{
	var socket = webSocketInstances[socketInstance];
	if (socket.messages.length == 0)
		return;
	if (socket.messages[0].length > leh)
		return;
	HEAPU8.set(socket.messages[0], ptr);
	socket.messages = socket.messages.slice(1);
},

TardisWebSocketClose: function (socketInstance)
{
	var socket = webSocketInstances[socketInstance];
	socket.socket.close();
}
};

autoAddDeps(LibraryTardisWebSockets, '$webSocketInstances');
mergeInto(LibraryManager.library, LibraryTardisWebSockets);
