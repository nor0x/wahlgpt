window.scrollToElement = function (selector) {
	var element = document.querySelector(selector);
	if (element) {
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

let suggestions = [
	"Was wird für Umweltschutz getan?",
	"Wie ist die Position zu Klimaschutz?",
	"Wie wird die Digitalisierung vorangetrieben?",
	"Wie steht die Partei zum Thema Bildung?",
	"Wie können Familien finanziell unterstützt werden?",
	"Gibt es Pläne für die Gesundheitsversorgung?",
	"Welche Pläne gibt es für die Pensionen?",
	"Wie ist die Position zu sozialen Themen?",
	"Welche Pläne gibt es für die Wirtschaft?",
	"Wie können Startups unterstützt werden?",
	"Wie ist die Position zu Europa und EU?",
	"Wie steht die Partei zu Migration?",
	"Welche Pläne gibt es für die Landwirtschaft?",
	"Wie ist die Position zu Sicherheit und Polizei?",
];

var questionInput = undefined;
window.doSuggestions = function () {
	questionInput = document.querySelector('.question-input');
	question = questionInput.value;
	if (question == '') {
		let placeholder = document.querySelector('.question-input');
		let index = Math.floor(Math.random() * suggestions.length);
		typeWriter(placeholder, suggestions[index], 0);

		setInterval(function () {
			if (questionInput && questionInput.value == '') {
				let placeholder = document.querySelector('.question-input');
				let index = Math.floor(Math.random() * suggestions.length);
				typeWriter(placeholder, suggestions[index], 0);
			}
		}, Math.floor(Math.random() * 3000) + 3000);
	}
}

let typeWriter = function (element, text, i) {
	if (i < text.length) {
		element.setAttribute('placeholder', text.substring(0, i + 1));
		setTimeout(function () {
			typeWriter(element, text, i + 1)
		}, Math.floor(Math.random() * 90) + 10);
	}
}

window.doCountUp = function (count) {

	const els = document.querySelectorAll('[data-countup]');

	els.forEach(el => {
		el.textContent = count;
	});
	els.forEach(makeCountup);
}



function countup(el, target) {
	let data = { count: 0 };
	anime({
		targets: data,
		count: [0, target],
		duration: 5000,
		round: 1,
		delay: 200,
		easing: 'easeOutCubic',
		update() {
			el.innerText = data.count.toLocaleString();
		}
	});
}

function makeCountup(el) {
	const text = el.textContent;
	const target = parseInt(text, 10);

	const io = new IntersectionObserver(entries => {
		entries.forEach(entry => {
			if (entry.intersectionRatio > 0) {
				countup(el, target);
				io.unobserve(entry.target);
			}
		});
	});

	io.observe(el);
}
