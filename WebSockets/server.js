const WebSocket = require("ws");

miservidor = new WebSocket.Server(
	{port:3000},
	aliniciar
);
function aliniciar() {
	console.log("servidor iniciado");
}

var conexiones=[];

miservidor.on("connection", alguienconectado);
function alguienconectado(conexionconcliente) {
	conexiones.push(conexionconcliente);
	console.log("nueva conexion: conectados "+conexiones.length);
	conexionconcliente.send("bienvenido");

	conexionconcliente.on("message", 
		function (data) {
			console.log(data.toString());
			//conexionconcliente.send(data.toString());
			var i = clamp(Math.floor(Math.random() * conexiones.length),0,1000)
            conexiones.forEach(c => {if(c == conexiones[i])conexiones[i].send(data.toString())});
		}
	);

	conexionconcliente.on("close", 
		function() {
			conexiones = conexiones.filter(
				(c)=>{
					return c!=conexionconcliente
				}
			);	
			console.log("conexion cerrara: conectados "+conexiones.length);
		}
	);
}
function getRandomInt(max) {
    return Math.floor(Math.random() * max);
  }

  const clamp = (val, min, max) => Math.min(Math.max(val, min), max)