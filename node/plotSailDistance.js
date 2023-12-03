const fs = require('fs');
const { plot } = require('nodeplotlib');

function calculateDistance(f, r, theta, t1) {
  // const cotTheta = 1 / Math.tan(theta);
  const distance = Math.sign(t1)*(Math.abs(f * Math.sin(theta)) - r); //abs(f + r * Math.sin(theta)) / Math.sqrt(cotTheta ** 2 + 1);
  return distance;
}

function drawFunction(f, r) {
  const theta = [];
  const distance = [];

  for (let t1 = -90; t1 <= 90; t1 += 1) {
    let t = (270 + t1)/180 * Math.PI;
    theta.push(t1);
    distance.push(calculateDistance(f, r, t, t1));
  }

  const trace = {
    x: theta,
    y: distance,
    type: 'scatter',
  };

  const layout = {
    xaxis: {
      title: 'Theta',
    },
    yaxis: {
      title: 'Distance (d)',
    },
    title: 'Distance from P1 to Tangent Line on Circle',
  };

  const figure = { data: [trace], layout };

  plot(figure, (err, imageData) => {
    if (err) {
      console.log('An error occurred while plotting:', err);
    } else {
      fs.writeFileSync('plot.png', imageData, 'base64');
      console.log('Plot created. You can view it at: plot.png');
    }
  });
}

// Example usage
const f = 6; // Value for f
const r = 3; // Value for r
drawFunction(f, r);