window.FileUtil = {
    saveAs: function (fileName, bytesBase64) {
        const link = document.createElement("a");
        link.download = fileName;
        link.href = "data:application/octet-stream;base64," + bytesBase64;
        document.body.appendChild(link); // Required for FireFox
        link.click();
        document.body.removeChild(link);
    }
};
