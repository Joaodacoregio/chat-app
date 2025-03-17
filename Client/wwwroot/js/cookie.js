window.setCookie = (name, value, days) => {
    var d = new Date();
    d.setTime(d.getTime() + (days * 24 * 60 * 60 * 1000)); // Expiração em dias
    var expires = "expires=" + d.toUTCString();
    document.cookie = name + "=" + value + ";" + expires + ";path=/;domain=192.168.1.162;samesite=lax";
};

function getCookie(name) {
    const value = `; ${document.cookie}`;
    const parts = value.split(`; ${name}=`);
    if (parts.length === 2) return parts.pop().split(';').shift();
}

window.removeCookie = (name) => {
    document.cookie = name + "=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/; domain=192.168.1.162; samesite=lax"; //Expira o cookie com data
};