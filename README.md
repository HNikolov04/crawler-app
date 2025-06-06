# HTML Crawler

This project represents one of my first major programming endeavors developed as a student project at Technical University of Sofia and serves as a testament to my early learning journey in software development. It demonstrates fundamental understanding of data structures, algorithms, and software architecture.

## 🎓 Project Origin
This project was developed as one of my first programming endeavors during my studies at Technical University of Sofia. It serves as a practical implementation of data structures and algorithms, specifically focusing on HTML document manipulation.

## 🚀 Features

- **HTML Parsing**: Parse and create tree structures from HTML documents
- **Tree Manipulation**: Various commands to modify and interact with the HTML structure
- **Image Support**: Handle HTML documents with embedded BMP images
- **File Operations**: Save and load functionality with custom compression
- **Parallel Processing**: Utilizes multi-threading for improved performance

## 📋 Available Commands

- `New` - Opens a new HTML file
- `Load` - Loads a previously saved file
- `Print` - Displays node contents
- `Set` - Modifies node content
- `Copy` - Copies nodes within the tree structure
- `Save` - Saves the current state with compression
- `Visualize` - Shows a visual representation of the HTML structure
- `ChangeFile` - Switch to a different file
- `Exit` - Close the application

## 🛠 Technical Implementation

The project is built using C# and implements several custom data structures:
- Custom Dictionary
- Custom Linked List
- Custom List
- Custom Stack

It also features:
- Huffman compression for file storage
- Parallel processing for tree operations
- Custom HTML parsing and tree building
- BMP image handling capabilities

## 🏗 Project Structure

The solution follows a clean architecture approach with three main projects:
- `Crawler.Domain` - Core entities and interfaces
- `Crawler.Application` - Business logic and services
- `Crawler.Presentation` - User interface and command handling