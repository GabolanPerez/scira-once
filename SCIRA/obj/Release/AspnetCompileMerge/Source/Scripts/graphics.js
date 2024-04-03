

function buildChart(data, type, title, canvas, options,backgroundColor) {
    options.title.text = title;

    if (backgroundColor != null) {
        var chartArea = { backgroundColor: backgroundColor }
        options.chartArea = chartArea;
    }

    var chart = new Chart(canvas, {
        type: type,
        data: data,
        options: options
    });

    return chart;
}

// Plugin para mostrar cantidades sobre las gráficas
Chart.plugins.register({
    afterDatasetsDraw: function (chart) {
        var ctx = chart.ctx;

        chart.data.datasets.forEach(function (dataset, i) {
            var meta = chart.getDatasetMeta(i);
            if (!meta.hidden) {
                meta.data.forEach(function (element, index) {
                    // Draw the text in black, with the specified font
                    ctx.fillStyle = 'rgb(0, 0, 0)';

                    var fontSize = 16;
                    var fontStyle = 'normal';
                    var fontFamily = 'Helvetica Neue';
                    ctx.font = Chart.helpers.fontString(fontSize, fontStyle, fontFamily);

                    //revisar si existe dataLabels dentro del dataset
                    var dataLabels = dataset.dataLabels;

                    if (dataLabels == undefined) {
                        // Just naively convert to string for now
                        var dataString = dataset.data[index].toString();
                    } else {
                        var dataString = dataLabels[index].toString();
                    }

                    

                    // Make sure alignment settings are correct
                    ctx.textAlign = 'center';
                    ctx.textBaseline = 'middle';

                    var padding = 5;
                    var position = element.tooltipPosition();
                    ctx.fillText(dataString, position.x, position.y - (fontSize / 2) - padding);
                });
            }
        });
    }
});

//Plugin para colorear gráficas
Chart.pluginService.register({
    beforeDraw: function (chart, easing) {
        if (chart.config.options.chartArea && chart.config.options.chartArea.backgroundColor) {
            var helpers = Chart.helpers;
            var ctx = chart.chart.ctx;
            var chartArea = chart.chartArea;

            ctx.save();
            ctx.fillStyle = chart.config.options.chartArea.backgroundColor;
            ctx.fillRect(chartArea.left, chartArea.top, chartArea.right - chartArea.left, chartArea.bottom - chartArea.top);
            ctx.restore();
        }
    }
});

//Plugin para dibujar lineas en la gráfica
Chart.pluginService.register({
    afterDraw: function(chartobj) {
        if (chartobj.options.lines) {
            var ctx = chartobj.chart.ctx;
            for (var idx = 0; idx < chartobj.options.lines.length; idx++) {
                var line = chartobj.options.lines[idx];
                line.iniCoord = [0, 0];
                line.endCoord = [0, 0];
                line.color = line.color ? line.color : "black";
                line.label = line.label ? line.label : "";
                if (line.type == "horizontal" && line.y) {
                    line.iniCoord[1] = line.endCoord[1] = chartobj.scales["y-axis-0"].getPixelForValue(line.y);
                    line.endCoord[0] = chartobj.chart.width;
                } else if (line.type == "vertical" && line.x) {
                    line.iniCoord[0] = line.endCoord[0] = chartobj.scales["x-axis-0"].getPixelForValue(line.x);
                    line.endCoord[1] = chartobj.chart.height;
                }
                ctx.beginPath();
                ctx.moveTo(line.iniCoord[0], line.iniCoord[1]);
                ctx.lineTo(line.endCoord[0], line.endCoord[1]);
                ctx.strokeStyle = line.color;
                ctx.stroke();
                ctx.fillStyle = line.color;
                ctx.fillText(line.label, line.iniCoord[0] + 3, line.iniCoord[1] + 3);
            }
        }
    }
});