    // Constants
    const KN = 10; // Constant for normal sail power
    const KT = 10; // Constant for tangential sail power
    const KW = 1; // Constant for angular change rate calculation
    const M = 10; // Boat mass in kg
    const L = 5; // Boat length in meters
    const KB = 0.5; // factorfor wind effect on the boat body;
    let DT = 1/60; // Time interval in seconds, but it is acrually compured in updateBoat()
    const R = [-1.0,0,0.5]; // Water resistance constant [backward, still, forward]
    const boatLength = 170;
    const mastPosition = 20;
    const sailLength = 110;
    const fSailLength = 80;
    const rudderLength = 40;
    
    // Variables
    let sailAngle = 30 * Math.PI  / 180 ;
    let steeringAngle = 0;
    let boatDirection = Math.PI/2; //Math.PI/4; // Angle in radians
    let boatSpeed = 0; // Meters per second
    let boatDrift = 0; // Meters per second boat drifting sideways.
    let windDirection = Math.PI/2; // Angle in radians
    let windSpeed = 30; // Meters per second
    let maxWindSpeed = 10; // Meters per second
    let keelPosition = 0; // 0 = up, 1 = middle, 2 = down
    let boatPosition = { x: 0, y: 0 }; // Boat position in meters
    // Canvas setup
    let canvas, ctx, centerX, centerY, boat_speed_text; // context variables to be set on initialization
    
    // Draw wind arrow
    // Draw wind arrow
function drawWind() {
    const arrowLength = windSpeed;
    const arrowAngle = windDirection; // Math.PI / 6;
    const size = Math.max(canvas.width, canvas.height) /2 - 35;

    const arrowX1 = centerX - size * Math.cos(-arrowAngle);
    const arrowY1 = centerY - size * Math.sin(-arrowAngle); 
    const arrowX2 = arrowX1 + arrowLength * Math.cos(arrowAngle);
    const arrowY2 = arrowY1 - arrowLength * Math.sin(arrowAngle);
    const headSize = Math.max(arrowLength / 3, 10);
    const xh1 = arrowX2 + headSize * Math.cos(arrowAngle + Math.PI*3/4);
    const yh1 = arrowY2 - headSize * Math.sin(arrowAngle + Math.PI*3/4);
    const xh2 = arrowX2 + headSize * Math.cos(arrowAngle - Math.PI*3/4);
    const yh2 = arrowY2 - headSize * Math.sin(arrowAngle - Math.PI*3/4);

    ctx.beginPath();
    ctx.linewidth = 5;
    // ctx.strokefill = 'blue';
    ctx.moveTo(arrowX1, arrowY1);
    ctx.lineTo(arrowX2, arrowY2);
    ctx.lineTo(xh1, yh1);
    ctx.moveTo(arrowX2, arrowY2);
    ctx.lineTo(xh2, yh2);
    ctx.stroke();    
}

// Draw boat
const pathData1 = `
M 0.163116,4.11449 L 0.894806,2.84715 L 1.34703,1.45537 L 1.5,-0 L 1.34703,-1.45537 L 0.894806,-2.84715
    L 0.163116,-4.11449 L 0,-4.29565 L -0.163116,-4.11449 L -0.894806,-2.84715 L -1.34703,-1.45537 L -1.5,-0
    L -1.34703,1.45537 L -0.894806,2.84715 L -0.163116,4.11449 L 0,4.29565 L 0,5 z
`;
const pathData = `
M 0.894806,2.84715 L 1.34703,1.45537 L 1.5,-0 L 1.34703,-1.45537 L 0.894806,-2.84715
    L 0.163116,-4.11449 L 0,-4.29565 L -0.163116,-4.11449 L -0.894806,-2.84715 L -1.34703,-1.45537 L -1.5,-0
    L -1.34703,1.45537 L -0.894806,2.84715  z
`;

function drawBoat() {
    ctx.save();
    ctx.scale(20.,20.);
    ctx.rotate(Math.PI/2-boatDirection);
    ctx.lineWidth = 0.1;
    const path = new Path2D(pathData);
    ctx.stroke(path);
    ctx.restore();
}
function drawFullBoat() {
    drawBoat();
    const boatX = centerX; // + boatSpeed * Math.cos(boatDirection);
    const boatY = centerY; // - boatSpeed * Math.sin(boatDirection);
    const sailX1 = boatX;
    let x = -Math.PI/2 + sailAngle
    const sailX2 = sailX1 + sailLength * Math.cos(x);
    const sailY1 = boatY - mastPosition;
    const sailY2 = sailY1 - sailLength * Math.sin(x);
    const fsailX1 = boatX;
    x = -Math.PI/2 + sailAngle
    const fsailX2 = fsailX1 + fSailLength * Math.cos(x);
    const fsailY1 = boatY - boatLength/2 + 10;
    const fsailY2 = fsailY1 - fSailLength * Math.sin(x);
    const rudderX1 = boatX;
    x = -Math.PI/2 + steeringAngle
    const rudderX2 = rudderX1 + rudderLength * Math.cos(x);
    const rudderY1 = boatY + boatLength/2 - 28;
    const rudderY2 = rudderY1 - rudderLength * Math.sin(x);
    ctx.beginPath();
    ctx.save();
    ctx.rotate(Math.PI/2-boatDirection);
    ctx.moveTo(sailX1, sailY1);
    ctx.lineTo(sailX2, sailY2);
    ctx.moveTo(fsailX1, fsailY1);
    ctx.lineTo(fsailX2, fsailY2);
    ctx.moveTo(rudderX1, rudderY1);
    ctx.lineTo(rudderX2, rudderY2);
    ctx.stroke();
    ctx.restore();
}
    
// Displaying data on screen methods:
function X(w,v, v2) {
    document.getElementById(w).value = (v/Math.PI * 180).toFixed(1) + 
        (v2 === undefined ? "" : (" / " + (v2/Math.PI * 180).toFixed(1)));
}
function Y(w,v, v2) {
    document.getElementById(w).value = v.toFixed(1) +
        (v2 === undefined ? "" : (" / " + v2.toFixed(1)));
}
const displayWindAngle = (v) => X('windAngle',v);
const displayWindSpeed = (v) => Y('windSpeed',v/10);
const displaySailAngle = (v) => X('sailAngle',v);
const displaySteeringAngle = (v) => X('steeringAngle',v);
const displayBoatDirection = (v) => X('boatDirection',v);
const displayBoatSpeed = (speedF, speedD) => Y('boatSpeed',speedF, speedD);
const displayBoatPosition = (posx, posy) => Y('boatPosition',posx, posy);
const displayWindBoatAngle = (v) => X('windBoatAngle',v);
const displayWindSailAngle = (v) => X('windSailAngle',v);
const displayForce = (fwdF, driftF) => Y('windForce',fwdF, driftF);
const displayWaterResistance = (v) => Y('waterResistance',v);
const displayAcceleration = (v) => Y('acceleration',v);
const displayRudderTorque = (v) => Y('rudderTorque',v);

// Update boat direction and speed
let boatIncrement = 0;
let lastTime = new Date().getTime();
function condition(v, min, max) {
    return v >= min && v <= max ? 1 : 0;
}
function updateBoat() {
    let now = new Date().getTime();
    DT = (now - lastTime)/1000;
    lastTime = now;
    // boatDirection = (boatDirection + boatIncrement)% (2*Math.PI);
    // Compute angle between wind and boat direction
    let windAngle = (windDirection - boatDirection);
    if (windAngle > Math.PI) windAngle = windAngle - 2*Math.PI; // wind to boat angle is positive on the right sie, negative on the left side.
    let windSailAngle = Math.sign(sailAngle) * (sailAngle - windAngle); // positive on the left sideof sail, negative on the right (back) side
    if (windSailAngle > Math.PI) windSailAngle = windSailAngle - 2*Math.PI;
    displayWindSailAngle(windSailAngle);
    displayWindAngle(windDirection); 
    displayWindSpeed(windSpeed);
    displayBoatDirection(boatDirection); 
    displayWindBoatAngle(windAngle);
    // Compute sail and rudder forces
    const sailForce = Math.sign(windSailAngle) * windSpeed * (
        KT * Math.abs(Math.cos(windSailAngle))* condition(windSailAngle, 140, 170) + 
        KN * Math.abs(Math.sin(windSailAngle))
    );
    const fwdSailForce = sailForce * Math.abs(Math.sin(sailAngle));
    const driftSailForce = sailForce * Math.abs(Math.cos(sailAngle));
    let fwdForce = fwdSailForce + windSpeed * Math.cos(windAngle) * KB;
    let driftForce = (driftSailForce + Math.sign(-windAngle) * windSpeed * Math.abs(Math.sin(windAngle)) * KB ) * [0.1, 0.05, 0.01][keelPosition];
    const waterResistance = -R[Math.sign(boatSpeed)+1] * boatSpeed * boatSpeed;
    displayForce(fwdForce, driftForce);
    displayWaterResistance(waterResistance);
    const steeringStep = (Math.PI/4/5); // 5 steps in steering from straight to 45 degrees. 
    // none linear steering factor. Slows down if above 45 degrees.
    const steeringFactor = [0,0.1,0.3,0.7,1.25, 0.8, 0.5, 0.1, 0][Math.min(8, Math.round(Math.abs(steeringAngle)/steeringStep))]; 
    const rudderForce = -KW * Math.abs(Math.sin(steeringAngle)) * boatSpeed * steeringFactor;
    const rudderTorque = -rudderForce * Math.sign(steeringAngle); // * Math.cos(steeringAngle);

    displayRudderTorque(rudderTorque);
    boatDirection = (boatDirection - rudderTorque / M * DT) % (2*Math.PI);
    // Compute boat acceleration
    let fwdAcceleration = (fwdForce + rudderForce + waterResistance) / M;
    // acceleration = Math.abs(acceleration) < 1 ? 0 : acceleration;
    displayAcceleration(fwdAcceleration);
    // Update boat speed and direction
    boatSpeed += fwdAcceleration * DT;
    boatDrift = driftForce / M;
    displayBoatSpeed(boatSpeed, boatDrift);
    boatPosition.x += boatSpeed * Math.cos(boatDirection) * DT + boatDrift * Math.sin(boatDirection) * DT;
    boatPosition.y += boatSpeed * Math.sin(boatDirection) * DT + boatDrift * Math.cos(boatDirection) * DT;
    displayBoatPosition(boatPosition.y, boatPosition.x);
    ctx.restore();
    ctx.save();
    ctx.rotate(boatDirection);

    drawCanvas2(boatPosition, boatDirection, [10., 10.], [canvas.width/2, canvas.height/2]); 
    //boatDirection += boatSpeed * Math.sin(windAngle) / L * DT;
}

// Update wind speed and direction
function updateWind() {
    // TODO: Implement wind simulation
}

// Update canvas
function updateCanvas() {
    displaySailAngle(sailAngle);
    displaySteeringAngle(steeringAngle);
    ctx.clearRect(minx, miny, maxx-minx, maxy-miny); //-canvas.width/2,-canvas.height/2, canvas.width, canvas.height);
    drawWind();
    drawFullBoat();
    updateWind();
    drawNESW(-boatDirection, ctx);
    drawMainGrid(-boatDirection);
    updateBoat();
}

let interval;
function stopStart(button) {
    if (button.innerHTML === "Stop") {
        button.innerHTML = "Resume";
        button.style.backgroundColor = "lightgreen";
        boatIncrement = 0;
    } else {
        button.innerHTML = "Stop";
        button.style.backgroundColor = "pink";
        // interval = setInterval(updateCanvas, 1000/60);
        boatIncrement = 0.001;
    }
}

// Create scratch canvas for rotated letters
const scratchCanvas = document.createElement('canvas');
scratchCanvas.width = 50; // width of each letter
scratchCanvas.height = 50; // height of each letter
const scratchCtx = scratchCanvas.getContext('2d');
scratchCtx.translate(scratchCanvas.width / 2, scratchCanvas.height / 2);
scratchCtx.font = '30px Arial';
scratchCtx.textAlign = 'center';
scratchCtx.textBaseline = 'middle';
scratchCtx.save();


function drawMainGrid(r) {
        r = (r === undefined ? Math.PI/4 : r) + Math.PI/2;
        // draw grid
        ctx.strokeStyle = 'blue';
        ctx.setLineDash([10,5]);
        ctx.lineWidth = 0.5;
        ctx.beginPath();
        const round = (p, s) => p - p % s
        let x= round(boatPosition.x, canvas.width) - boatPosition.x;  
        let y= round(-boatPosition.y, canvas.height) + boatPosition.y;  
        let minx = x - canvas.width*3/2, maxx = x + canvas.width*3/2,
            miny = y - canvas.height*3/2, maxy = y + canvas.height*3/2;
        for (let x= minx + canvas.width/2; x < maxx; x += canvas.width/2) {
        for (let y= miny + canvas.width/2; y < maxy; y += canvas.height/2) {
                ctx.moveTo(x, miny);
                ctx.lineTo(x, maxy);
                ctx.moveTo(minx, y);
                ctx.lineTo(maxx, y);
            }
        }
        ctx.stroke();    
}
// draw NESW letters and circle and lines marking 4 additional directions, and cross-lines to represent water
// relative to boat as it moves.
function drawNESW(r, ctx) {
    r = (r === undefined ? Math.PI/4 : r) + Math.PI/2;
    // Draw rotated letters on scratch canvas and copy to main canvas
    const letterPositions = [
        { l: "N", x: 0, y: -canvas.height/2 + 20 }, // N
        { l: "E", x: canvas.width/2 - 20, y: 0 }, // E
        { l: "S", x: 0, y: canvas.height/2 -20 }, // S
        { l: "W", x: -canvas.width/2 + 20, y: 0 } // W
    ];

    letterPositions.forEach((position) => {
        scratchCtx.clearRect(-scratchCanvas.width/2, -scratchCanvas.height/2, scratchCanvas.width, scratchCanvas.height);
        scratchCtx.rotate(r);
        scratchCtx.fillText(position.l, 0, 0);
        ctx.drawImage(scratchCanvas, position.x - scratchCanvas.width/2, position.y - scratchCanvas.height/2);
        scratchCtx.restore();
        scratchCtx.save();
    });
    // Draw circle and lines
    ctx.lineWidth = 0.5;
    ctx.setLineDash([10, 5]); // Set line dash pattern
    ctx.beginPath();
    ctx.arc(0, 0, canvas.width/2 - 5, 0, 2 * Math.PI); // Draw circle
    ctx.stroke();

    ctx.setLineDash([5,0]); // Restore line to solid
    ctx.beginPath();
    let s = Math.min(canvas.width/2, canvas.height/2) - 20;
    for (let x=Math.PI/4; x<2*Math.PI; x+=Math.PI/2) {
        ctx.moveTo(s * Math.cos(x), -s * Math.sin(x));
        ctx.lineTo((s+15) * Math.cos(x), -(s+15) * Math.sin(x));
    }
    ctx.stroke();
}

let ctx2, canvas2;
function initCanvas2() {
    // Get a reference to the canvas element
    canvas2 = document.getElementById("canvas2");
    // Get the 2D drawing context for the canvas
    ctx2 = canvas2.getContext("2d");

    ctx2.translate(canvas2.width / 2, canvas2.height / 2);
}

function drawCanvas2(position, direction, factor, gapSize) {  // position has x: and y: attributes, factor is panning factor.
    ctx2.clearRect(-canvas2.width/2,-canvas2.height/2, canvas2.width, canvas2.height); //, ctx.width, ctx.height);
    // Draw the grid lines
    ctx2.strokeStyle = "blue"; // set the color of the grid lines
    ctx2.lineWidth = 0.5
    ctx2.setLineDash([3, 5]); // set the line dash pattern to create dotted lines
    for (var i = -canvas2.width / 2 + gapSize[0] / factor[0]; i < canvas2.width / 2; i += gapSize[0] / factor[0]) {
        ctx2.beginPath();
        ctx2.moveTo(i, -canvas2.height / 2);
        ctx2.lineTo(i, canvas2.height / 2);
        ctx2.stroke();
    }
    for (var i = -canvas2.height / 2 + gapSize[1] / factor[1]; i < canvas2.height / 2; i += gapSize[1] / factor[1]) {
        ctx2.beginPath();
        ctx2.moveTo(-canvas2.width / 2, i);
        ctx2.lineTo(canvas2.width / 2, i);
        ctx2.stroke();
    }

    // Draw the marker
    ctx2.fillStyle = "#f00"; // set the color of the marker
    ctx2.beginPath();

    // console.log(position.x / factor[0] + 5*Math.cos(direction), -(position.y / factor[1] + 5*Math.sin(direction)));
    // console.log(position.x /factor[0] - 5 * Math.cos(direction + Math.PI / 5), 
    //     -(position.y/factor[1] - 5 * Math.sin(direction + Math.PI / 5)));
    // console.log(position.x / factor[0] - 5 * Math.cos(direction - Math.PI / 5),
    //     -(position.y/factor[1] - 5 * Math.sin(direction - Math.PI / 5)));
    const k = 20;
    ctx2.moveTo(position.x / factor[0] + k/2*Math.cos(direction), -(position.y / factor[1] + k/2*Math.sin(direction)));
    ctx2.lineTo(position.x /factor[0] - k/2 * Math.cos(direction + Math.PI / 5), 
        -(position.y/factor[1] - k/2 * Math.sin(direction + Math.PI / 5)));
    ctx2.lineTo(position.x / factor[0] - k/2 * Math.cos(direction - Math.PI / 5),
        -(position.y/factor[1] - k/2 * Math.sin(direction - Math.PI / 5)));
    ctx2.closePath();
    ctx2.fill();
    ctx2.stroke();
    drawNESW(-Math.PI/2, ctx2);
}
let minx, miny, maxx, maxy; // size of full drawing area
function init() {
    // Canvas setup
    canvas = document.getElementById('canvas');
    ctx = canvas.getContext('2d');
    ctx.translate(canvas.width / 2, canvas.height / 2);
    minx = -canvas.width * 10, maxx = canvas.width * 10, 
    miny = -canvas.height * 10, maxy = canvas.height * 10; 

    centerX = 0; // canvas.width / 2;
    centerY = 0; // canvas.height / 2;
    ctx.rotate(-Math.PI/2);
    ctx.save();
    ctx.rotate(boatDirection);
    boat_speed_text = document.getElementById('boat-speed-text');

    // Event listeners for sliders
    document.getElementById('sail-angle').addEventListener('input', function(event) {
        sailAngle = event.target.value * Math.PI / 180;
    });

    document.getElementById('steering-angle').addEventListener('input', function(event) {
        steeringAngle = event.target.value * Math.PI / 180;
    });

    document.getElementById('wind-direction').addEventListener('input', function(event) {
        windDirection = event.target.value * Math.PI / 180;
    });

    document.getElementById('wind-speed').addEventListener('input', function(event) { // alert("wind speed");
        windSpeed = event.target.value * maxWindSpeed;
    });

    document.getElementById('keel-position').addEventListener('input', function(event) { // alert("wind speed");
        keelPosition = event.target.value;
    });

    initCanvas2();

    // animation loop:  
    interval = setInterval(updateCanvas, 1000/60);
}
