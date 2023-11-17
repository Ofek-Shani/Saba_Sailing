const fs = require('fs');
const cheerio = require('cheerio');
const markdownIt = require('markdown-it');

const inputFile = 'README.md';
const outputFile = 'README.html';

// Read the Markdown file
const markdown = fs.readFileSync(inputFile, 'utf-8');

// Convert Markdown to HTML
const md = new markdownIt();
const html = md.render(markdown).replace(/&lt;/g, "<").replace(/&gt;/g, ">").replace(/&quot;/g, '"').replace("<table>", "<table border='1'>");
const $ = cheerio.load(html);
let $atag = $('<a>'); 
$atag.attr('href', "#toc");
$atag.append('&#8593;');
$('h2').each((index, element) => {
    const $heading = $(element);
    const origText = $heading.text();
    let replaceText = origText;
    if (!origText.startsWith("Table")) {
        // $heading.text("");
        $heading.attr('id', origText.toLowerCase());
        $heading.html($atag.toString() + " " + replaceText);
        // $heading.append(" " + origText);
    } else {
        $heading.attr('id', "toc");
    }
    const s = $heading.toString();
    console.log(s);
    console.log($heading.html());
// $heading.text(replaceText);
})

const modifiedHtml = $.html();
// Write the HTML to the output file
fs.writeFileSync(outputFile, modifiedHtml);

console.log(`Successfully converted ${inputFile} to ${outputFile}`);
