async function scan() {
    const video = document.getElementById('video');
    const canvas = document.getElementById('canvas');
    const context = canvas.getContext('2d');
    const displaySize = { width: 300, height: 200 };

    canvas.width = displaySize.width;
    canvas.height = displaySize.height;

    try {
        video.style.display = 'block';
        canvas.style.display = 'block';

        const stream = await navigator.mediaDevices.getUserMedia({
            video: { facingMode: "user" } // Use the front camera
        });

        video.srcObject = stream;
        video.play();

        decodeQRCode(video, canvas, context);
    } catch (error) {
        console.error("Error accessing the camera", error);
        alert('Error accessing the camera.');
    }
}

function decodeQRCode(video, canvas, context) {
    const scan = () => {
        context.drawImage(video, 0, 0, canvas.width, canvas.height);
        const imageData = context.getImageData(0, 0, canvas.width, canvas.height);
        const code = jsQR(imageData.data, imageData.width, imageData.height);

        if (code) {
            console.log("Found QR code", code.data);
            video.srcObject.getTracks().forEach(track => track.stop());
            video.style.display = 'none';
            canvas.style.display = 'none';
            sendQRCodeData(code.data);
            alert('QR Code data: ' + code.data);
            // You can handle the QR code data here, e.g., redirecting to a URL or making a server request
        } else {
            requestAnimationFrame(scan); // Keep scanning
        }
    };

    requestAnimationFrame(scan);
}

function sendQRCodeData(qrCodeData) {
    // Make an AJAX request to the C# controller
    const xhr = new XMLHttpRequest();
    xhr.open('POST', '/Home/ProcessQRData', true); // Adjust the endpoint URL as per your controller route
    xhr.setRequestHeader('Content-Type', 'application/json');
    xhr.onreadystatechange = function () {
        if (xhr.readyState === XMLHttpRequest.DONE) {
            if (xhr.status === 200) {
                console.log('QR code data sent successfully');
            } else {
                console.error('Error sending QR code data:', xhr.status);
            }
        }
    };
    xhr.send(JSON.stringify({ qrCodeData: qrCodeData }));
}