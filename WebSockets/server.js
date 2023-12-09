const WebSocket = require("ws");

miservidor = new WebSocket.Server(
	{port:3000},
	aliniciar
);
function aliniciar() {
	let dict = new Map();
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
			FilterMessage(data);
			/*

			console.log(data.toString());
			//conexionconcliente.send(data.toString());
			var i = clamp(Math.floor(Math.random() * conexiones.length),0,1000)
            conexiones.forEach(c => {if(c == conexiones[i])conexiones[i].send(data.toString())});
			console.log(a["action"]);*/
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

// *** Reading Message ***

function FilterMessage(data){
var Json = JSON.parse(data.toString());

console.log(Json);

switch(Json["action"]){
	case 'SendBlock':
		SendBlock(data);
	break;
	case '':
	break;
	default:
	console.log("El mensaje no se pudo identificar la accion");
	break;
}
}

// *** Respose Functions ***

function SendBlock(data){
	console.log("Se mando un bloque");
	var i = clamp(Math.floor(Math.random() * conexiones.length),0,1000)
	conexiones.forEach(c => {if(c == conexiones[i])conexiones[i].send(data.toString())});

	data.toString
}

// *** Math Functions ***

function getRandomInt(max) {
    return Math.floor(Math.random() * max);
  }

  const clamp = (val, min, max) => Math.min(Math.max(val, min), max)

// *** Dictionary ***

function learnDictionary(key, result){
	dict.set(key, result);
}