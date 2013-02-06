(function ()
{
	var x, y,
		context,
		started,
		textstart,

		penGreen = function ()
		{
			context.lineWidth = 5;
			context.strokeStyle = "rgba(0,255,0,0.1)";
		},

		penRed = function ()
		{
			context.lineWidth = 10;
			context.strokeStyle = "rgba(255,0,0,0.5)";
		},

		penBlue = function ()
		{
			context.lineWidth = 15;
			context.strokeStyle = "rgba(0,0,255,0.5)";
		},

		penBlack = function ()
		{
			context.lineWidth = 5;
			context.strokeStyle = "rgba(0,0,0,0.5)";
		};

	$("body").append("<canvas id=\"canvasOverlay\" style=\"position:absolute;top:0px;z-index:100000;cursor:crosshair;\" >Your browser cannot view this page.</canvas>");
	
	context = $("#canvasOverlay")[0].getContext("2d");
	context.canvas.width = window.innerWidth;
	context.canvas.height = window.innerHeight;
	context.lineCap = "round";
	context.lineJoin = "round";
	context.strokeStyle = "#000";
	context.stroke();

	$("#canvasOverlay").mousemove(function (ev)
	{
		if (ev.layerX || ev.layerX == 0)
		{ // Firefox
			x = ev.layerX;
			y = ev.layerY;
		}
		else if (ev.offsetX || ev.offsetX == 0)
		{ // Opera
			x = ev.offsetX;
			y = ev.offsetY;
		}

		if (!started)
		{
			context.moveTo(x, y);
			context.beginPath();
		}
		else
		{
			context.lineTo(x, y);
			context.stroke();
		}
	});

	$("#canvasOverlay").mousedown(function (ev)
	{
		if (ev.layerX || ev.layerX == 0)
		{ // Firefox
			x = ev.layerX;
			y = ev.layerY;
		} else if (ev.offsetX || ev.offsetX == 0)
		{ // Opera
			x = ev.offsetX;
			y = ev.offsetY;
		}

		if (textstart)
		{
			context.fillText("joriN", x, y);
			textstart = false;
		}
		else
		{
			context.beginPath();
			context.moveTo(x, y);
			started = true;
		}
	});

	$("#canvasOverlay").mouseup(function ()
	{
		started = false;
		context.save();
	});

	$(window).keyup(function (ev)
	{
		if (ev.keyCode == 49)
		{
			penGreen();
		}
		else if (ev.keyCode == 50)
		{
			penRed();
		}
		else if (ev.keyCode == 51)
		{
			penBlue();
		}
		else if (ev.keyCode == 52)
		{
			penBlack();
		}
		else if (ev.keyCode == 8)
		{
			context.restore();
		}
		else if (ev.keyCode == 84)
		{
			textstart = true;
		}
	}).keydown(function (ev)
	{
		if (ev.keyCode == 8)
		{
			ev.preventDefault();
		}
	}).resize(function (ev)
	{
		context.canvas.width = window.innerWidth;
		context.canvas.height = window.innerHeight;
	});

	alert('done');


}).call(this);