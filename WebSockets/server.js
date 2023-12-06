const WebSocket = require("ws");

const miServidor = new WebSocket.Server({port:3000}, () => {
    console.log("Servidor WebSocket iniciado en el puerto 3000...")
});

var conexiones = [];

miServidor.on("connection", (socket) =>{
    conexiones.push[socket];

        console.log("alguien se ha conectado, ya somos "+conexiones.length);
    socket.send("Hola eres el wacho: " + conexiones.length);

    socket.on("close", ()=>{
        conexiones.filter((c)=>{c!=socket});
        conexiones.forEach((s)=>s.send("Alguien se ha ido"));
        console.log("alguien ha cerrado las conexiones");
    });

    socket.on("message", (datos)=>{
        console.log("Me han enviado esto: "+ datos.toString())
        socket.send(datos.toString());
        //conexiones[getRandomInt(conexiones.length - 1)].send(datos)
        //conexiones.forEach((s)=>{if(s!=socket)s.send(datos.toString())});
    })
})

function getRandomInt(max) {
    return Math.floor(Math.random() * max);
  }