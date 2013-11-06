(function() {
    // consts
    var SERIES_LIMIT = 200;
    var MAX_CONSOLE_LENGTH = 500;

    // controls
    var consolePlaceholder = document.getElementById("console");
    var statePlaceholder = document.getElementById("state");
    var avgPlaceholder = document.getElementById("avg");
    var minPlaceholder = document.getElementById("min");
    var maxPlaceholder = document.getElementById("max");
    var recentPlaceholder = document.getElementById("recent");
    var countPlaceholder = document.getElementById("count");

    // globals
    var seriesData = [{x:0,y:10}];
    var averageData = [{x:0,y:10}];
    var recentData = [{x:0,y:10}];
    var cnt = 0;

    // initialization
    var socket = initSocket();
    var updateStateChart = initChart("state_chart", seriesData);
    var updateAverageChart = initChart("average_chart", averageData);
    var updateRecentChart = initChart("recent_chart", recentData);

    function initSocket() {
        var socket = new WebSocket('ws://localhost:8181');
        socket.onopen = function() {
            console.log('handshake successfully established. May send data now...');
        };

        socket.onmessage = function(event) {
            var data = JSON.parse(event.data);
            
            appendConsole(data.Data);
            
            updateCurrentBlock(data);

            updateDataSeries(data.Data, seriesData);
            updateDataSeries(data.Stats.AverageRoundtrip, averageData);
            updateDataSeries(data.Stats.RecentRoundtrip, recentData);

            updateStateChart();
            updateAverageChart();
            updateRecentChart();

            socket.send(event.data); // return the message back
        };
        socket.onclose = function() {
            console.log('connection closed');
        };
        socket.onerror = function(err) {
            console.log('unexpected error happened: ' + err);
        };
        return socket;
    }

    function appendConsole(data) {
        var html = consolePlaceholder.innerHTML;
        var trunkatedHtml = html.length > MAX_CONSOLE_LENGTH ? html.substring(html.length - MAX_CONSOLE_LENGTH, html.length) : html; // silly trunkate method. don't use in production!
        trunkatedHtml += data + '<br>';
        consolePlaceholder.innerHTML = trunkatedHtml;
        consolePlaceholder.scrollTop = consolePlaceholder.scrollHeight;
    }

    function updateCurrentBlock(data) {
        statePlaceholder.innerHTML = data.Data;
        avgPlaceholder.innerHTML = data.Stats.AverageRoundtrip;
        minPlaceholder.innerHTML = data.Stats.MinRoundtrip;
        maxPlaceholder.innerHTML = data.Stats.MaxRoundtrip;
        recentPlaceholder.innerHTML = data.Stats.RecentRoundtrip;
        countPlaceholder.innerHTML = data.Stats.Count;
    }

    function updateDataSeries(data, arr) {
        arr.push({x:cnt,y:parseInt(data)});
        if(arr.length > SERIES_LIMIT) {
            arr.splice(0, 1);
        }
        cnt++;
    }

    function initChart(elementId, data) {
        var palette = new Rickshaw.Color.Palette( { scheme: 'classic9' } );

        // instantiate our graph!

        var graph = new Rickshaw.Graph( {
            element: document.getElementById(elementId),
            width: 500,
            height: 100,
            renderer: 'area',
            stroke: true,
            preserve: true,
            series: [
                {
                    color: 'lightgreen',
                    data: data,
                    name: 'Data'
                }
            ]
        } );

        graph.render();

        var hoverDetail = new Rickshaw.Graph.HoverDetail( {
            graph: graph,
            xFormatter: function(x) {
                return new Date(x * 1000).toString();
            }
        } );

        var ticksTreatment = 'glow';
        var xAxis = new Rickshaw.Graph.Axis.Time( {
            graph: graph,
            min: "auto",
            max: "auto"
        } );

        xAxis.render();

        var yAxis = new Rickshaw.Graph.Axis.Y( {
            graph: graph,
            tickFormat: Rickshaw.Fixtures.Number.formatKMBT,
            ticksTreatment: ticksTreatment
        } );

        yAxis.render();

        return function() {
            graph.update();
        };
    }
    
})();