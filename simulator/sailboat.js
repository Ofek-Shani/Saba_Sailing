    // Constants
    const K = 0.1; // Constant for boat speed calculation
    const KW = 1; // Constant for angular change rate calculation
    const M = 200; // Boat mass in kg
    const L = 5; // Boat length in meters
    const DT = 1/60; // Time interval in seconds
    const boatLength = 170;
    const mastPosition = 20;
    const sailLength = 110;
    const fSailLength = 80;
    const rudderLength = 40;
    
    // Variables
    let sailAngle = 30 * Math.PI  / 180 ;
    let steeringAngle = 0;
    let boatDirection = 0; //Math.PI/4; // Angle in radians
    let boatSpeed = 0; // Meters per second
    let windDirection = Math.PI/2; // Angle in radians
    let windSpeed = 30; // Meters per second
    let maxWindSpeed = 10; // Meters per second
    // Canvas setup
    let canvas, ctx, centerX, centerY, boat_speed_text; // context variables to be set on initialization
    
    // Draw wind arrow
    // Draw wind arrow
function drawWind() {
    const arrowLength = windSpeed;
    const arrowAngle = windDirection; // Math.PI / 6;
    const size = Math.max(canvas.width, canvas.height) /2 - 10;

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
const pathData = `
M 0.163116,4.11449 L 0.894806,2.84715 L 1.34703,1.45537 L 1.5,-0 L 1.34703,-1.45537 L 0.894806,-2.84715
    L 0.163116,-4.11449 L 0,-4.29565 L -0.163116,-4.11449 L -0.894806,-2.84715 L -1.34703,-1.45537 L -1.5,-0
    L -1.34703,1.45537 L -0.894806,2.84715 L -0.163116,4.11449 L 0,4.29565 z
`;

function drawBoat() {
    ctx.save();
    ctx.scale(20.,20.);
    ctx.rotate(boatDirection + Math.PI/2);
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
    const rudderY1 = boatY + boatLength/2;
    const rudderY2 = rudderY1 - rudderLength * Math.sin(x);
    ctx.beginPath();
    ctx.save();
    ctx.rotate(boatDirection + Math.PI/2);
    ctx.moveTo(sailX1, sailY1);
    ctx.lineTo(sailX2, sailY2);
    ctx.moveTo(fsailX1, fsailY1);
    ctx.lineTo(fsailX2, fsailY2);
    ctx.moveTo(rudderX1, rudderY1);
    ctx.lineTo(rudderX2, rudderY2);
    ctx.stroke();
    ctx.restore();
}
    
function X(w,v) {
    document.getElementById(w).value = (v/Math.PI * 180).toFixed(2);
}
const A = (v) => X('A',v);
const B = (v) => X('B',v);
const C = (v) => X('C',v);
// Update boat direction and speed
function updateBoat() {
    boatDirection = (boatDirection += 0.001)% (2*Math.PI);
    // Compute angle between wind and boat direction
    let windAngle = -(windDirection - boatDirection);
    if (windAngle > Math.PI) windAngle = windAngle - 2*Math.PI;
    A(windDirection); B(boatDirection); C(windAngle);
    // Compute sail and rudder forces
    const sailForce = K * Math.cos(sailAngle + windAngle);
    const rudderForce = KW * Math.sin(steeringAngle) * (1 - Math.exp(-K * boatSpeed));
    // Compute boat acceleration
    const acceleration = (sailForce + rudderForce) / M;
    // Update boat speed and direction
    boatSpeed += acceleration * DT;
    //boatDirection += boatSpeed * Math.sin(windAngle) / L * DT;
    boat_speed_text.innerHTML = boatSpeed.toFixed(2);
}

// Update wind speed and direction
function updateWind() {
    // TODO: Implement wind simulation
}

// Update canvas
function updateCanvas() {
    ctx.clearRect(-canvas.width/2,-canvas.height/2, canvas.width, canvas.height);
    drawWind();
    drawFullBoat();
    updateBoat();
    updateWind();
}

let interval;
function stopStart(button) {
    if (button.value === "Stop") {
        button.value = "Start";
        button.style.backgroundColor = "green";
        clearInterval(interval);
    } else {
        button.value = "Stop";
        button.style.backgroundColor = "pink";
        interval = setInterval(updateCanvas, 1000/60);
    }
}
function init() {
    // Canvas setup
    canvas = document.getElementById('canvas');
    ctx = canvas.getContext('2d');
    ctx.translate(canvas.width / 2, canvas.height / 2);
    // ctx.rotate(-Math.PI/2);
    centerX = 0; // canvas.width / 2;
    centerY = 0; // canvas.height / 2;
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

    // animation loop:  
    interval = setInterval(updateCanvas, 1000/60);
}
