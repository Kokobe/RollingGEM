var express			= require('express');
var app				= express();
var server			= require('http').createServer(app);
var io 				= require('socket.io').listen(server);
var shortId 		= require('shortid');


app.set('port', process.env.PORT || 2559);

var games = {};
var winner = "";
var readyPlayers = 0;

io.on('connection', function (socket) {

	socket.on('USER_CONNECT', function (){
		console.log('User Connected ');
	});

	socket.on('JOIN GAME', function (data) {
		var game_pin = data["game_pin"]
		if (Object.keys(games).includes(game_pin)){
			games[game_pin]["players"] += 1;
 		} else {
			games[game_pin] = {"players" : 1, "ready players" : 0, "winner" : ""};
		}
		socket.join(game_pin);
		console.log("room created: " + game_pin)
		console.log("num of clients in room = " + io.sockets.adapter.rooms[game_pin].length);
		io.to(game_pin).emit("USER JOINED GAME", games[game_pin]);
		if(games[game_pin]["players"] > 1) {
			io.to(game_pin).emit("CONNECTION READY");
		} else {
			io.to(game_pin).emit("CONNECTION NOT READY");
		}
	});

	socket.on('READY TO PLAY', function (data){
		var game_pin = data["game_pin"]
		games[game_pin]["ready players"] += 1;
		if (games[game_pin]["ready players"] > 1)
			io.to(game_pin).emit("BOTH PLAYERS READY", {});
		console.log("ready players = " + games[game_pin]["ready players"]);
	});

	socket.on('RESET READY PLAYERS', function (data){
		var game_pin = data["game_pin"]
		games[game_pin]["ready players"] = 0;
		games[game_pin]["winner"] = "";
		console.log("ready players = " + games[game_pin]["ready players"]);
	});

	socket.on('FINISHED', function (data){
		var game_pin = data["game_pin"]
		if (games[game_pin]["winner"] === ""){
			games[game_pin]["winner"] = data.winner;
		}	
		data = {
			winner: games[game_pin]["winner"]
		}
		io.to(game_pin).emit("RESULT", data);
	});

	socket.on('disconnect', function (){
		Object.keys(games).forEach(game_pin => {
			var clients = io.sockets.adapter.rooms[game_pin];
			if (clients == null){
				console.log("num of clients in room = " + 0);
				delete games[game_pin]
			} else if (clients.length != games[game_pin].players) {
				games[game_pin]["players"] = clients.length;
				games[game_pin]["ready players"] = 0;
				io.to(game_pin).emit("USER LEFT GAME", games[game_pin]);
				if(games[game_pin]["players"] <= 1) {
					io.to(game_pin).emit("CONNECTION NOT READY", {});
				}
			}
		});
	});
});


server.listen( app.get('port'), function (){
	console.log("------- server is running -------");
} );

