// wwwroot/scripts/chat.js
window.scrollToBottom = (element) => {
    console.log("Elemento recebido:", element);
    console.log("scrollHeight:", element.scrollHeight);
    console.log("scrollTop antes:", element.scrollTop);
    element.scrollTop = element.scrollHeight;
    console.log("scrollTop depois:", element.scrollTop);
    return true;
};