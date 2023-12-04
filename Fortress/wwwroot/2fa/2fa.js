let form = document.getElementById("2fa-form")

form.onsubmit = async (ev) => {
	ev.preventDefault()

	let response = await fetch(form.action, {
		method: "patch",
		body: new FormData(form),
		headers: { "Content-Type": "application/json" }
	})
	
	let status = response.status
	let bearerToken = await response.text()
	
	if (response.status === 200) {
		document.location.assign("/home")
	}
	else if (response.status === 400) {
		let errorDialog = document.getElementById("2fa-error")
		
		errorDialog.show()
	}
}
