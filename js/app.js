window.scrollToElement=function(a){var b=document.querySelector(a);if(b){let a=document.getElementById("one");a.style.height="auto",a.style.opacity="1",a.style.padding="6em 0 4em 0",a.style.background="white",b.scrollIntoView({behavior:"smooth"})}},window.copyToClipboard=function(a){navigator.clipboard.writeText(a),alert("Email Adresse kopiert: "+a)};