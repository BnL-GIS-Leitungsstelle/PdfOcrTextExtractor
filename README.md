# PdfOcrTextExtractor

The PdfOcrTextExtractor is a simple Console Application which extracts all the text from pdf files in a given input folder and stores it in an sqlite database. It does so by first converting all pages of the pdfs to images and then extracts the text using OCR. 

##  Getting started
1. Go to the Program.cs file and change the `inputFolder` variable to the desired folder containing the pdf files.
2. Also adjust the `outputSqlite` variable to the desired output sqlite file path.
3. Compile + Run.
