RandomNames = [
    "Nestor",
    "Elvina",
    "Maurine",
    "Kaycee",
    "Penni",
    "Laticia",
    "Brianne",
    "Madelyn",
    "Irene",
    "Margy",
    "Enrique",
    "Shae",
    "Avril",
    "Bridgette",
    "Carlena",
    "Celestina",
    "Elina",
    "Trena",
    "Alita",
    "Annita",
    "Dortha",
    "Melissia",
    "Nydia",
    "Lucia",
    "Darcie",
    "Kimbra",
    "Annette",
    "Jolynn",
    "Sylvester",
    "Krystyna",
];
SharpMemoUI = {
    args: {},
    tableId: null,
    geometry: {},
    tableState: {},
    
    mePlayer: {
        screenName: "someblock",
        sessionId: "my-session"
    },
    
    index: 0,
    
    loadTables: function(destination) {
        $.ajax({
            url: "/sharp-memo/v1/tables"
        })
            .done(function(data) {
                let tables = data;

                for(let i = 0; i < tables.length; i++) {
                    destination.append("<div><a href='table.html#tableId=" + tables[i] + "' >" + tables[i] + "</a></div>");
                }
            });
    },
    
    loadTable: function(args) {
        let randomNumber = Math.floor(Math.random() * RandomNames.length);
        SharpMemoUI.mePlayer.screenName = RandomNames[ randomNumber ];
        SharpMemoUI.mePlayer.sessionId = "session-" + randomNumber;
        
        SharpMemoUI.args = args;
        SharpMemoUI.tableId = args.tableId;
        SharpMemoUI.geometry = args.geometry;
        
        SharpMemoUI.setupJoinButtonArrea(args);
        SharpMemoUI.setupGuessArea(args);
        SharpMemoUI.setupMemoArea(args);
        
        SharpMemoUI.updateTableState(args);
    },

    setupJoinButtonArrea: function(args) {
        args.joinButtonArrea.empty().append("Join as: " + SharpMemoUI.mePlayer.screenName);
        args.joinButtonArrea.click(() => {
            SharpMemoUI.joinTable();
        });
    }, 
    
    setupGuessArea: function(args) {
        args.guessArea.click((a) => {
            let guessAreaOffset = args.guessArea.offset();
            let left = a.pageX - guessAreaOffset.left;
            let top = a.pageY - guessAreaOffset.top;
            
            let row = parseInt(top / ( args.geometry.height / args.geometry.rows ));
            let column = parseInt(left / ( args.geometry.width / args.geometry.columns ));
            let nextGuess = (row * args.geometry.columns + column);
            
            console.log(row + "x" + column + " = " + nextGuess);

            SharpMemoUI.nextGuess(args, nextGuess);
        });
    },
    
    setupMemoArea: function(args) {
        for( let column = 0; column < args.geometry.columns; column ++ ) {
            args.memoArea.append(
                "<div id='memo-" + column + "' class='memo-cell'></div>"
            );
        }
    },
    
    nextGuess: function(args, guessId) {
        // SharpMemoUI.updateMemoCell(SharpMemoUI.index % SharpMemoUI.geometry.columns, guessId);
        // SharpMemoUI.index++;
        $.ajax({
            url: "/sharp-memo/v1/table/" + SharpMemoUI.tableId + "/guess",
            type: "POST",
            processData: false,
            contentType: "application/json",
            data: JSON.stringify({ player: SharpMemoUI.mePlayer, guess: guessId}),
        }).done((data) => {
            // SharpMemoUI.updateTableState(SharpMemoUI.args);
        });
    },
    
    updateMemoCell: function(memoColumn, guessId) {
        let row = Math.floor(guessId / SharpMemoUI.geometry.columns);
        let column = guessId % SharpMemoUI.geometry.columns;
        
        $('#memo-'+memoColumn).css({
            backgroundPositionX: (-Math.floor(SharpMemoUI.geometry.width / SharpMemoUI.geometry.columns) * column) + "px",
            backgroundPositionY: (-Math.floor(SharpMemoUI.geometry.height / SharpMemoUI.geometry.rows) * row) + "px",
            opacity: 1.0,
        });
    },

    updateTableState: function(args) {
        args.state = SharpMemoUI.loadTableState(args.tableId, (state) => {
            if (state.timestamp != SharpMemoUI.lastTimestamp) {
                SharpMemoUI.lastTimestamp = state.timestamp;       
                SharpMemoUI.updatePlayers(args, state);
                SharpMemoUI.updateMemo(args, state);
            }
            if (SharpMemoUI.updateTimeoutId) {
                window.clearTimeout(SharpMemoUI.updateTimeoutId)
            }
            SharpMemoUI.updateTimeoutId = window.setTimeout(() => {
                SharpMemoUI.updateTableState(SharpMemoUI.args);
            }, 100);
        }, SharpMemoUI.tableState.timestamp );
    },

    loadTableState: function(tableId, onDone, knownTimestamp) {
        $.ajax({
            url: "/sharp-memo/v1/table/" + tableId + (knownTimestamp ? "/" + new Date(knownTimestamp).getTime() : "")
        })
            .done(function(data) {
                SharpMemoUI.tableState = data;
                
                // console.log(SharpMemoUI.tableState);

                onDone(SharpMemoUI.tableState);
            });
    },

    updatePlayers: function (args, tableState)  {
        args.playersArea.empty();
        for( let p in tableState.players) {
            let player = tableState.players[p];
            args.playersArea.append("<div>" + player.screenName + " </div>");
        }
    },

    updateMemo: function (args, tableState) {
        let memo = tableState.memo;
        let reset = tableState.guessPosition == 0;
        let endPosition = reset ? memo.length : tableState.guessPosition;
        let startPosition = Math.max(endPosition-6, 0);
        
        let column = 0;
        for( var position = startPosition; position < endPosition; position++) {
            SharpMemoUI.updateMemoCell(column, memo[position]);
            column++;
        }
        
        if (reset) {
            window.setTimeout(() => {
                let column = 0;
                while (column < 6) {
                    $('#memo-'+column).css({ opacity: 0.0 });
                    column++;
                }
            }, 1000);
        }
    },
    
    joinTable: function() {
        $.ajax({
            url: "/sharp-memo/v1/table/" + SharpMemoUI.tableId + "/join",
            type: "POST",
            processData: false,
            contentType: "application/json",
            data: JSON.stringify({ player: SharpMemoUI.mePlayer })
        }).done((data) => {
            // SharpMemoUI.updateTableState(SharpMemoUI.args);
        });
    }
}