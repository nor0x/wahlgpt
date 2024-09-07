window.scrollToElement = function (selector) {
	var element = document.querySelector(selector);
	if (element) {
		//animate height of #one to 200px

		let one = document.getElementById('one');
		one.style.height = 'auto';
		one.style.opacity = '1';
		one.style.padding = '6em 0 4em 0';
		one.style.background = 'white';

		element.scrollIntoView({ behavior: 'smooth' });
	}
}

window.copyToClipboard = function (email) {
	navigator.clipboard.writeText(email);
	alert('Email Adresse kopiert: ' + email);
}

