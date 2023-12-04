let form = document.getElementById("2fa-form")

form.onsubmit = async (ev) => {
	ev.preventDefault()

	let response = await fetch(form.target, {
		method: form.getAttribute("method"),
		body: new FormData(form)
	})
	
	let status = response.status
	let bearerToken = await response.text()
	
	if (response.status === 200) {
		document.location.assign("https://localhost:5000/fortress/home")
	}
	else if (response.status === 400) {
		let errorDialog = document.getElementById("2fa-error")
		
		errorDialog.show()
	}
}
