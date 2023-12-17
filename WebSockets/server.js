const WebSocket = require("ws");

let publicRoom = new Map();
let ConexionRoom = new Map();
let TemporizadorRoom = new Map();
let TemporizadorRoomNumber = new Map();
let StateRoom = new Map();
let ScoreRoom = new Map();
let FinishListRoom = new Map();
let LoserListRoom = new Map();

var MaxPlayers = 99;
var Timer = 2;

const ServerState = {
	Offline: 0,
	Countdown: 1,
	FindingPlayers: 2,
	Ingame: 3,
	Closing: 4
  };

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

	if(conexiones.length >= 0 && conexiones.length <= 10){
		conexiones.push(conexionconcliente);
		conexiones.forEach(c => {if(!ConexionRoom.has(c)){SendUsersConnected(c)}});
	}else{
		conexiones.push(conexionconcliente);
	}
	
	console.log("nueva conexion: conectados "+conexiones.length);
	conexionconcliente.send("bienvenido");

	conexionconcliente.on("message", 
		function (data) {
			FilterMessage(data, conexionconcliente);
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
			
			if(conexiones.length >= 0 && conexiones.length <= 10){
				SendUsersConnected(conexionconcliente);
				conexiones.forEach(c => {if(!ConexionRoom.has(c)){SendUsersConnected(c)}});
			}

			console.log("conexion cerrara: conectados "+conexiones.length);
			console.log(publicRoom.has(ConexionRoom.get(conexionconcliente)));

			LeavePublicRoom(conexionconcliente);
		}
	);

	conexionconcliente.on("ping", 
	function(data) {
		if(data.toString() != ""){
			SendPing( data, conexionconcliente);
		}
		}
	);
}

// *** Reading Message ***

function FilterMessage(data, conexiontemporal){
var Json = JSON.parse(data.toString());

console.log(Json);

switch(Json["action"]){
	case 'SendBlock':
		SendBlock(data);
	break;
	case 'JoinPublicRoom':
		JoinPublicRoom(data, conexiontemporal);
	break;
	case 'LeavePublicRoom':
		LeavePublicRoom(conexiontemporal);
	break;
	case 'GetPlayerNumber':
		SendUsersConnected(conexiontemporal);
	break;
	case 'StartMatch':
		StartMatch(Json["roomID"]);
	break;
	case 'BlocksRemain':
		var JsonScore = JSON.parse(Json["Json"]);
		agregarPuntuacion(conexiontemporal, JsonScore["Value1"].toString(), Json["roomID"].toString());
	break;
	case 'ILost':
		agregarPerdido(conexiontemporal, 0, Json["roomID"].toString());
	break;
	case '':
	break;
	default:
	console.log("No se pudo identificar la accion");
	break;
}
}

// *** Respose Functions ***

function JoinPublicRoom(data, conexiontemporal){
	var Json = JSON.parse(data.toString());
	if(Json["roomID"].toString() != "")
	return;

	var TempRoom = "pub00";
	var Connected = false;

	for (const [clave, valor] of publicRoom){

		if (publicRoom.get(clave).length >= MaxPlayers) {
			continue;
		}

		if(!StateRoom.has(clave)){
			continue;
		}

		if((StateRoom.get(clave) != ServerState.FindingPlayers && StateRoom.get(clave) != ServerState.Countdown && StateRoom.get(clave) != ServerState.Offline) && StateRoom.has(clave)){
			continue;
		}

		TempRoom = clave;
		Json["roomID"] = TempRoom;
		Connected = true;

		break;
	}

	if(Connected == false){
		console.log("Entro");
		TempRoom = CreatePublicRoom();
		Json["roomID"] = TempRoom;
	}

	publicRoom.get(TempRoom).push(conexiontemporal);

	conexiontemporal.send(JSON.stringify(Json));
	ConexionRoom.set(conexiontemporal, TempRoom);
	SendPlayerCounterRoom(TempRoom);
	console.log("Unido al Lobby eres el " + publicRoom.get(TempRoom).length);
	console.log(publicRoom.get(TempRoom).length);

	if(publicRoom.get(TempRoom).length >= MaxPlayers){
	StartMatch(TempRoom);
	}else{
		var obj = new Object();
		obj.action = "TimerStarter";
		obj.roomID = TempRoom;
		obj.user = " ";
	
		var obj2 = new Object();
		obj2.Value1 = TemporizadorRoomNumber.get(TempRoom);
		obj.Json = JSON.stringify(obj2);
	
		conexiontemporal.send(JSON.stringify(obj));
	}
}

function CreatePublicRoom(){
for(var i = 1; i < 99;i++){
	serie = i.toString().padStart(2, '0');
	if(publicRoom.has("pub"+serie)){
		continue;
	}
	console.log("Lobby con id " + "pub"+serie + " creado")
	CreateRoom("pub"+serie);
	StateRoom.set("pub"+serie, ServerState.FindingPlayers);
	StartTemporizador("pub"+serie);

	var scores = [];
	ScoreRoom.set("pub"+serie, scores);
	var Players = [];
	FinishListRoom.set("pub"+serie, Players);
	var Loosers = [];
	LoserListRoom.set("pub"+serie, Loosers);

	return ("pub"+serie);
}
return null;
}

function LeavePublicRoom(conexiontemporal){
	if (!publicRoom.has(ConexionRoom.get(conexiontemporal))) {
		return;
	}

	var conexionesTemporales = [];
	var roomID = ConexionRoom.get(conexiontemporal);

	conexionesTemporales = publicRoom.get(roomID).filter(
		(c) => {
			return c != conexiontemporal
		}
	);

	if (conexionesTemporales.length == 0) {
		publicRoom.delete(roomID);
		StateRoom.delete(roomID);
		ScoreRoom.delete(roomID);
		ScoreRoom.delete(roomID);
		FinishListRoom.delete(roomID);
		LoserListRoom.delete(roomID);
		DetenerTemporizador(roomID);
	} else {
		publicRoom.set(roomID, conexionesTemporales);
		SendPlayerCounterRoom(roomID);
		obtenerPuntajeParaCadaJugador(roomID);
		scores = ScoreRoom.get(roomID);

		scores = scores.filter(puntuacion => puntuacion.playerName !== conexiontemporal);

		ScoreRoom.set(roomID, scores);
		obtenerPuntajeParaCadaJugador(roomID);
	}

	ConexionRoom.delete(conexiontemporal);
}

function CreateRoom(ID){
	publicRoom.set(ID, []);
}

function StartTemporizador(ID){
	let tiempoRestante = Timer;
	TemporizadorRoomNumber.set(ID, tiempoRestante);
	const tareaInterval = setInterval(() => {

		if (tiempoRestante <= 0) {
			TemporizadorRoomNumber.clear(ID);
			clearInterval(tareaInterval);
			StateRoom.set(ID, ServerState.FindingPlayers);

			console.log('¡Cuenta regresiva finalizada!');

			var obj = new Object();
			obj.action = "TimerEnded";
			obj.roomID = ID;
			obj.user = " ";
		
			publicRoom.get(ID).forEach(c => {
				c.send(JSON.stringify(obj));
			});

		} else {
			console.log(`Tiempo restante: ${tiempoRestante} segundos`);
			tiempoRestante--;
			TemporizadorRoomNumber.set(ID, tiempoRestante);
		}
	  }, 1000);

	TemporizadorRoom.set(ID, tareaInterval);
}

function StartMatch(ID){
	if(publicRoom.get(ID).length <= 1)
	return;

	var obj = new Object();
	obj.action = "StartMatch";
	obj.roomID = ID;
	obj.user = " ";

	DetenerTemporizador(ID);

	publicRoom.get(ID).forEach(c => {
		c.send(JSON.stringify(obj));
	});
	
	StateRoom.set(ID, ServerState.Ingame);
}

function SendPlayerCounterRoom(ID){
	var obj = new Object();
	obj.action = "SetLocalPlayerCount";
	obj.roomID = ID;
	obj.user = " ";

	var obj2 = new Object();
	obj2.Value1 = publicRoom.get(ID).length + + FinishListRoom.get(ID).length;
	obj.Json = JSON.stringify(obj2);

	publicRoom.get(ID).forEach(c => {
		c.send(JSON.stringify(obj));
		console.log("Mande el objeto");
	});
}

function DetenerTemporizador(ID){

	if (TemporizadorRoom.has(ID)) {
		const tareaInterval = TemporizadorRoom.get(ID);
		clearInterval(tareaInterval);
		TemporizadorRoom.delete(ID);
	}

}

function SendBlock(data){
	var Json = JSON.parse(data.toString());
	if(Json["roomID"].toString() == "")
	return;

	console.log("Se mando un bloque");

	scores = ScoreRoom.get(Json["roomID"].toString());

	if (scores.length > 1) {
		var i = clamp(Math.floor(Math.random() * scores.length),0,scores.length)
		scores[i].playerName.send(data.toString())
		AddPuntosAjugador(scores[i].playerName,1,Json["roomID"].toString());
		obtenerPuntajeParaCadaJugador(Json["roomID"].toString());
	}

	data.toString
}

function FinishMatch(ID,conexiontemporal){

	var obj = new Object();
	obj.action = "FinishMatch";
	obj.roomID = ID;
	obj.user = " ";

	var obj2 = new Object();
	var index = 1;
	obj2.Value1 = null;
	
	FinishListRoom.get(ID).forEach(c => {
		if(c != conexiontemporal){
			index++;
			return;
		}
		console.log("Lo encontre")
		obj2.Value1 = index;
	})
	if(obj2.Value1 === null){
		console.log("Es nulo")
		obj2.Value1 = 1 + ScoreRoom.get(ID).length + FinishListRoom.get(ID).length;
	}

	obj.Json = JSON.stringify(obj2);

	conexiontemporal.send(JSON.stringify(obj));
}

function agregarPerdido(playerName, score, ID){
	var scores = ScoreRoom.get(ID);

	var jugador = scores.find(puntuacion => {
		return puntuacion.playerName == playerName;
	});

	if (jugador != null) {
		scores = scores.filter(
			(c)=>{
				return c!=jugador
			}
		);
		console.log(scores.length)
	}
	scores.sort((a, b) => a.score - b.score);
	ScoreRoom.set(ID, scores);
	LoserListRoom.get(ID).push(playerName);
	FinishMatch(ID, playerName);

	if(ScoreRoom.get(ID).length <= 1){
		ScoreRoom.get(ID).forEach(c => {
			FinishListRoom.get(ID).push(c.playerName);
			FinishMatch(ID, c.playerName);
		})
	}
	else{
		obtenerPuntajeParaCadaJugador(ID);
	}

}

function agregarPuntuacion(playerName, score, ID) {

	var scores = ScoreRoom.get(ID);

	var jugador = scores.find(puntuacion => {
		return puntuacion.playerName == playerName;
	});


	if (jugador != null) {
		scores = scores.filter(
			(c)=>{
				return c!=jugador
			}
		);
	}

	if(score <= 0){
		ScoreRoom.set(ID, scores);
		FinishListRoom.get(ID).push(playerName);
		FinishMatch(ID, playerName);

		if(ScoreRoom.get(ID).length <= 1){
			ScoreRoom.get(ID).forEach(c => {
				FinishListRoom.get(ID).push(c.playerName);
				FinishMatch(ID, c.playerName);
			})
		}
		else{
			obtenerPuntajeParaCadaJugador(ID);
		}
	}else{
		scores.push({ playerName, score });
		console.log(scores.length);
		ScoreRoom.set(ID, scores);
		obtenerPuntajeParaCadaJugador(ID);
	}
}

function obtenerPuntajeDeJugador(playerName, ID) {
	scores = ScoreRoom.get(ID);
	scores.sort((a, b) => b.score - a.score);
	const jugador = scores.find((puntuacion) => puntuacion.playerName === playerName);
	if(jugador != null){

		var obj = new Object();
		obj.action = "SetTop";
		obj.roomID = ID;
		obj.user = " ";
	
		var obj2 = new Object();
		obj2.Value1 = jugador.score;
		obj.Json = JSON.stringify(obj2);

		jugador.playerName.send(JSON.stringify(obj.toString()))
	}
}

function obtenerPuntajeParaCadaJugador(ID) {

	scores = ScoreRoom.get(ID);

	if(scores.length <= 0 || scores == null)
	return;

	scores.sort((a, b) => a.score - b.score);

	var index = FinishListRoom.get(ID).length + 1;

	scores.forEach((puntuacion) => {
		console.log("Estoy recorriendo la lista " + index);

		var obj = new Object();
		obj.action = "SetTop";
		obj.roomID = ID;
		obj.user = " ";
	
		var obj2 = new Object();
		obj2.Value1 = index;
		obj.Json = JSON.stringify(obj2);

		puntuacion.playerName.send(JSON.stringify(obj));
		index++;
	});

	return null;
}


function restarPuntosAjugador(playerName, puntosARestar, ID) {
	scores = ScoreRoom.get(ID);
	var jugador = scores.find(puntuacion => {
		return puntuacion.playerName == playerName;
	});


	if (jugador != null) {
		jugador.score -= puntosARestar;

		if (jugador.score < 0) {
			jugador.score = 0;
		}
		console.log("Le reste puntos");
	} else {
		console.log(`El jugador ${playerName} no fue encontrado.`);
	}
}

function AddPuntosAjugador(playerName, puntosARestar, ID) {
	scores = ScoreRoom.get(ID);
	var jugador = scores.find(puntuacion => {
		return puntuacion.playerName == playerName;
	});


	if (jugador != null) {
		IntScore = parseInt(jugador.score);
		jugador.score = IntScore + puntosARestar;

		if (jugador.score < 0) {
			jugador.score = 132;
		}
		console.log("Le reste puntos");
	} else {
		console.log(`El jugador ${playerName} no fue encontrado.`);
	}
}

function SendPing(data, conexiontemporal){
	const PingDate = new Date(data.toString());
	const fechaActual = new Date();
	const Ping =  PingDate - fechaActual;

	var obj = new Object();
	obj.action = "SetPing";
	obj.roomID = " ";
	obj.user = " ";

	var obj2 = new Object();
	obj2.Value1 = clamp(Ping,0,999);

	obj.Json = JSON.stringify(obj2);

	conexiontemporal.send(JSON.stringify(obj));
}

function SendUsersConnected(conexiontemporal){
	var obj = new Object();
	obj.action = "GetPlayerNumber";
	obj.roomID = " ";
	obj.user = " ";

	var obj2 = new Object();
	obj2.Value1 = conexiones.length;

	obj.Json = JSON.stringify(obj2);
	conexiontemporal.send(JSON.stringify(obj));
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