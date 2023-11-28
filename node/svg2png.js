const fs = require('fs');
const svg2img = require('svg2img');

// Function to convert SVG to PNG
function convertSvgToPng(svgFilePath, pngFilePath, callback) {
  fs.readFile(svgFilePath, 'utf-8', (err, svgData) => {
    if (err) {
      callback(err);
      return;
    }

    svg2img(svgData, { format: 'png', width: 300, height: 300 }, (err, buffer) => {
      if (err) {
        callback(err);
        return;
      }

      fs.writeFile(pngFilePath, buffer, (err) => {
        if (err) {
          callback(err);
          return;
        }

        callback(null);
      });
    });
  });
}

// Example usage
const svgFilePath = '../list.svg';
const pngFilePath = '../list.png';

convertSvgToPng(svgFilePath, pngFilePath, (err) => {
  if (err) {
    console.error('Error:', err);
    return;
  }

  console.log('SVG converted to PNG successfully!');
});
