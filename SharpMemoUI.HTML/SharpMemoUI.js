SharpMemoUI = {
    args: {},
    tableId: null,
    geometry: {},
    tableState: {},
    
    mePlayer: {
        ScreenName: "someblock",
        SessionId: "my-session"
    },
    
    index: 0,
    
    loadTables: function(destination) {
        $.ajax({
            url: "http://localhost:8000/sharp-memo/v1/tables"
        })
            .done(function(data) {
                let tables = JSON.parse(data);

                for(let i = 0; i < tables.length; i++) {
                    destination.append("<div><a href='table.html#tableId=" + tables[i] + "' >" + tables[i] + "</a></div>");
                }
            });
    },
    
    loadTable: function(args) {
        SharpMemoUI.args = args;
        SharpMemoUI.tableId = args.tableId;
        SharpMemoUI.geometry = args.geometry;
        
        SharpMemoUI.setupJoinButtonArrea(args);
        SharpMemoUI.setupGuessArea(args);
        SharpMemoUI.setupMemoArea(args);
        
        SharpMemoUI.updateTableState(args);
    },

    setupJoinButtonArrea: function(args) {
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
        SharpMemoUI.updateMemoCell(SharpMemoUI.index % SharpMemoUI.geometry.columns, guessId);
        SharpMemoUI.index++;
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
            SharpMemoUI.updatePlayers(args, state);
            SharpMemoUI.updateMemo(args, state);         
        });
    },

    loadTableState: function(tableId, onDone) {
        $.ajax({
            url: "http://localhost:8000/sharp-memo/v1/table/" + tableId
        })
            .done(function(data) {
                SharpMemoUI.tableState = JSON.parse(data);
                
                console.log(SharpMemoUI.tableState);

                onDone(SharpMemoUI.tableState);
            });
    },

    updatePlayers: function (args, tableState)  {
        args.playersArea.empty();
        for( let p in tableState.Players) {
            args.playersArea.append("<div>" + p.ScreenName + " </div>");
        }
    },

    updateMemo: function (args, tableState) {
        
    },
    
    joinTable: function() {
        $.ajax({
            url: "http://localhost:8000/sharp-memo/v1/table/" + SharpMemoUI.tableId + "/join",
            type: "POST",
            processData: false,
            contentType: "application/json"
        }).done((data) => {
            SharpMemoUI.updateTableState(SharpMemoUI.args);
        });
    }
}