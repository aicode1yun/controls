let recognition = null;

export function startListening(dotnetRef, culture, continuous) {
    if (!('webkitSpeechRecognition' in window) && !('SpeechRecognition' in window)) {
        dotnetRef.invokeMethodAsync('OnSpeechError', 'SpeechRecognition not supported');
        return;
    }

    const SpeechRecognition = window.SpeechRecognition || window.webkitSpeechRecognition;
    recognition = new SpeechRecognition();
    recognition.continuous = continuous || false;
    recognition.interimResults = false;

    if (culture) {
        recognition.lang = culture;
    }

    recognition.onresult = (event) => {
        let transcript = '';
        for (let i = event.resultIndex; i < event.results.length; i++) {
            if (event.results[i].isFinal) {
                transcript += event.results[i][0].transcript;
            }
        }
        if (transcript) {
            dotnetRef.invokeMethodAsync('OnSpeechResult', transcript.trim());
        }
    };

    recognition.onend = () => {
        recognition = null;
        dotnetRef.invokeMethodAsync('OnSpeechEnd');
    };

    recognition.onerror = (event) => {
        recognition = null;
        dotnetRef.invokeMethodAsync('OnSpeechError', event.error);
    };

    recognition.start();
}

export function stopListening() {
    if (recognition) {
        recognition.stop();
        recognition = null;
    }
}
