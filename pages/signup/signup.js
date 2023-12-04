let form = document.getElementById("signup-form")


form.onsubmit = async (ev) => {
	ev.preventDefault()

	let response = await fetch(form.target, {
		method: form.getAttribute("method"),
		body: new FormData(form)
	})

	let id = await response.text()
	
	if (response.status === 200) {
		document.location.assign("https://localhost:5000/fortress/2fa")
	}
	else if (response.status === 400) {
		
	}
}
