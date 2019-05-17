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
    mode: 'observer',
    
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

    joinTable: function(args) {
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

    doJoinTable: function() {
        $.ajax({
            url: "/sharp-memo/v1/table/" + SharpMemoUI.tableId + "/join",
            type: "POST",
            processData: false,
            contentType: "application/json",
            data: JSON.stringify({ player: SharpMemoUI.mePlayer })
        }).done((data) => {
            // do nothing
        });
    },

    setupJoinButtonArrea: function(args) {
        args.joinButtonArrea.empty().append("Join as: " + SharpMemoUI.mePlayer.screenName);
        args.joinButtonArrea.click(() => { SharpMemoUI.doJoinTable(); });
    },

    doNextGuess: function(args, guessId) {
        $.ajax({
            url: "/sharp-memo/v1/table/" + SharpMemoUI.tableId + "/guess",
            type: "POST",
            processData: false,
            contentType: "application/json",
            data: JSON.stringify({ player: SharpMemoUI.mePlayer, guess: guessId}),
        }).done((data) => {
            // do nothing
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

            SharpMemoUI.doNextGuess(args, nextGuess);
        });
    },
    
    setupMemoArea: function(args) {
        for( let column = 0; column < args.geometry.columns; column ++ ) {
            args.memoArea.append(
                "<div id='memo-" + column + "' class='memo-cell'></div>"
            );
        }
    },
    
    // =================================================================================================================

    updateTableState: function(args) {
        SharpMemoUI.loadTableState(args.tableId, (state) => {
            if (state.timestamp != SharpMemoUI.tableState.timestamp) {
                SharpMemoUI.updatePlayers(args, state);
                SharpMemoUI.updateMemo(args, state);
                SharpMemoUI.checkGameOver(args, state);
            }
            SharpMemoUI.tableState = state;
            
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
            .done((state) => { onDone(state); });
    },

    // =================================================================================================================
    
    updatePlayers: function (args, tableState)  {
        args.playersArea.empty();
        for( let p in tableState.players) {
            let player = tableState.players[p];
            args.playersArea.append("<div>" + player.screenName + " </div>");
        }
    },

    displayGuess: function(memoColumn, guessId) {
        let row = Math.floor(guessId / SharpMemoUI.geometry.columns);
        let column = guessId % SharpMemoUI.geometry.columns;
        
        console.log( "displayGuess( " + memoColumn + ", " + guessId + ")");

        $('#memo-'+memoColumn)
            .removeClass("fadeOut")
            .css({
                backgroundPositionX: (-Math.floor(SharpMemoUI.geometry.width / SharpMemoUI.geometry.columns) * column) + "px",
                backgroundPositionY: (-Math.floor(SharpMemoUI.geometry.height / SharpMemoUI.geometry.rows) * row) + "px",
                opacity: 1.0,
            });

        window.setTimeout(() => {
            $('#memo-'+memoColumn)
                .addClass("fadeOut")
                .css({
                    opacity: 0.0
                });
        }, 500);
    },
    
    updateMemo: function (args, tableState) {
        console.log("updateMemo(" + tableState.memo + ") - guessPosition = " + tableState.guessPosition);
        
        let memo = tableState.memo;
        let reset = (tableState.guessPosition == 0) && (memo.length > 0);
        let endPosition = reset ? memo.length : tableState.guessPosition;
        let startPosition = Math.max(endPosition-SharpMemoUI.geometry.columns, 0);
        
        let column = 0;
        for( var position = startPosition; position < endPosition; position++) {
            SharpMemoUI.displayGuess(column, memo[position]);
            column++;
        }
        
        if (reset) {
            $(document.body)
                .addClass("flashGreen");
        } 
        else {
            $(document.body)
                .removeClass("flashRed")
                .removeClass("flashGreen");
        }
    },
    
    checkGameOver: function (args, tableState) {
        switch(SharpMemoUI.mode) {
            case 'observer' : 
                if (tableState.players.find( x => x.screenName == SharpMemoUI.mePlayer.screenName) != null ) {
                    SharpMemoUI.mode = 'player';
                }
                break; 
            case 'player' :
                if (tableState.players.find( x => x.screenName == SharpMemoUI.mePlayer.screenName) == null ) {
                    SharpMemoUI.mode = 'observer';
                    $(document.body).addClass("flashRed");
                }
                break;
        }
    }
}