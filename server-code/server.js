var express			= require('express');
var app				= express();
var server			= require('http').createServer(app);
var io 				= require('socket.io').listen(server);
var shortId 		= require('shortid');

var games = {};

app.set('port', process.env.PORT || 2559);

function randInt(min, max) { //min = inclusive, max = exclusive
	return min + Math.floor((max-min)* Math.random());
}

function randFloat(min, max) { 
	return min + (max-min)* Math.random();
}

function generateMazeDimensions () {
	var arr = [];
	for (var i = 0; i < 11; i++) {
		var randInteger = randInt(0, 3);
		switch (randInteger) {
			case 0:
				arr.push(randFloat(-0.5, -2.8));
				break;
			case 1:
				arr.push(randFloat(0.5, 2.8));
				break;
			case 2:
				var gap_space = randFloat(1, 2);
				arr.push([gap_space, randFloat(-0.3, -2.8)])
				break;
		}
	}
	return arr;
}

io.on('connection', function (socket) {

	socket.on('USER_CONNECT', function (){
		console.log('User Connected ');
	});

	socket.on('CHECK GAME PIN', function (data){
		var game_pin = data["game_pin"];
		var userId = data["userId"];
		if (Object.keys(games).includes(game_pin)) {
			io.emit("VALID GAME PIN", {"game_pin" : game_pin, "userId" : userId});
		} else {
			io.emit("INVALID GAME PIN", {"game_pin" : game_pin, "userId" : userId});
		}
	});

	socket.on('JOIN GAME', function (data) {
		var game_pin = data["game_pin"]
		if (Object.keys(games).includes(game_pin)){
			games[game_pin]["players"] += 1;
			console.log("room created: " + game_pin)
 		} else {
			games[game_pin] = {"players" : 1, "ready players" : 0, "winner" : ""};
			console.log("room joined: " + game_pin)
		}
		socket.join(game_pin);
		
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
		if (games[game_pin]["ready players"] === games[game_pin]['players']) {
			var game_dimensions = generateMazeDimensions();
			io.to(game_pin).emit("BOTH PLAYERS READY", {"game_dimensions" : game_dimensions});
			games[game_pin]["ready players"] = 0;
			games[game_pin]["winner"] = "";
		}
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
		if (games[game_pin]["players"] > 1) {
			io.to(game_pin).emit("CONNECTION READY");
		}
	});

	socket.on('LEAVE GAME', function (data){
		var game_pin = data["game_pin"]
		socket.leave(game_pin);
		var clients = io.sockets.adapter.rooms[game_pin];
		if (clients == null) {
			games[game_pin]["players"] = 0;
		} else {
			games[game_pin]["players"] = clients.length;
		}
		games[game_pin]["ready players"] = 0;
		io.to(game_pin).emit("USER LEFT GAME", games[game_pin]);
		if(games[game_pin]["players"] == 0) {
			delete games[game_pin]
		} else if(games[game_pin]["players"] == 1) {
			io.to(game_pin).emit("CONNECTION NOT READY", {});
		}
	});

	// can't get any data from 'disconnect' so i have to comb through everybody
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

