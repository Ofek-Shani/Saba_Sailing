const canvas = document.getElementById('canvas');
const ctx = canvas.getContext('2d');
function init() {
  canvas.width = window.innerWidth;
  canvas.height = window.innerHeight;
  alert("starting");
}
// Define boat and wind objects
const boat = {
  x: canvas.width / 2,
  y: canvas.height / 2,
  angle: 0,
  sailAngle: 0,
  speed: 0,
  draw: function() {
    // Draw boat on canvas using SVG
  },
  update: function() {
    // Update boat position and speed based on wind and steering angle
  }
};

const wind = {
  x: canvas.width,
  y: canvas.height / 2,
  angle: 0,
  power: 0,
  draw: function() {
    // Draw wind arrow on canvas
  },
  update: function() {
    // Update wind power and angle based on slider input
  }
};

// Update canvas every frame
function update() {
  // Clear canvas
  ctx.clearRect(0, 0, canvas.width, canvas.height);

  // Update boat and wind positions
  boat.update();
  wind.update();

  // Draw boat and wind on canvas
  boat.draw();
  wind.draw();

  // Request next frame
  requestAnimationFrame(update);
}

// Handle slider input
document.getElementById('steering-slider').addEventListener('input', function() {
  boat.angle = this.value;
});

document.getElementById('sail-slider').addEventListener('input', function() {
  boat.sailAngle = this.value;
});

// Start animation loop
update();