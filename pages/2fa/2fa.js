let form = getElementById("2fa-form")

form.onsubmit = async (ev) => {
	ev.preventDefault()

	let response = await fetch(form.target, {
		method: form.method,
		body: new FormData(form)
	})
	
	let status = response.status
	let bearerToken = await response.text()
	
	if (result.status === 200) {
		document.location.href.assign("https://localhost:5000/fortress/home")
	}
	else if (result.status === 400) {
		let errorDialog = getElementById("2fa-error")
		
		errorDialog.show()
	}
}
