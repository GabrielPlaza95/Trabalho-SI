let form = document.getElementById("signup-form")


form.onsubmit = async (ev) => {
	ev.preventDefault()

	let response = await fetch(form.action, {
		method: "post",
		body: new FormData(form),
		headers: { "Content-Type": "application/json" }
	})

	let id = await response.text()
	
	if (response.status === 200) {
		document.location.assign("/2fa")
	}
	else if (response.status === 400) {
		
	}
}
