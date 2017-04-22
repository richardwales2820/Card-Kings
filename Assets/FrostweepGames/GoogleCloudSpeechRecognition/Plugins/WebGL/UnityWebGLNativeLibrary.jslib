var UnityWebGLNativeLibrary =
{
$MicrophoneWebGLModuleNative: {},
	
DoMicrophoneLibByState: function(state) 
{		
		switch(state) 
		{
			case 1:
				StopRecord();
				break;
			case 0:
				StartRecord();
				break;				
			case 2:
				CheckMicrophoneAuthorization();
				break;
			default:
				console.log('state undefined ' + state);
				break;
		}

		function CheckMicrophoneAuthorization()
		{
			if(!IsMicrophoneAPIAvailable())
			{
				console.log('Microphone Lib is not supported in this browser');
				SendMessage('MicrophoneWebGLModuleNative', 'CheckMicrophoneAuthorizationHandler', 0);	
			}
			else if (!navigator.getUserMedia)
			{
				navigator.getUserMedia = navigator.getUserMedia || navigator.webkitGetUserMedia || navigator.mozGetUserMedia || navigator.msGetUserMedia;
				SendMessage('MicrophoneWebGLModuleNative', 'CheckMicrophoneAuthorizationHandler', 1);
			}
			else
			{
				console.log('Microphone Lib is already initialized');
			}
		}
		
		function IsMicrophoneAPIAvailable()
		{
			return !!(navigator.getUserMedia || navigator.webkitGetUserMedia || navigator.mozGetUserMedia || navigator.msGetUserMedia);
		}

		function StartRecord()
		{
			if(!NativeMicrophoneLibModule.audioContext)
			{
				if (navigator.getUserMedia) {
					navigator.getUserMedia({
							audio: true
						},
						GetUserMediaSuccess, 
						GetUserMediaFailed
					);
				}	
			}
			else
			{
				NativeMicrophoneLibModule.microphone_stream.connect(NativeMicrophoneLibModule.script_processor_node);
			}
		}
		
		function MicrophoneProcess(event)
		{
			var microphone_output_buffer = event.inputBuffer.getChannelData(0);
			
			for(var i = 0; i < microphone_output_buffer.length; i++)
			{
				SendMessage('MicrophoneWebGLModuleNative', 'AddByteToBufferFromMicrophoneHandler', microphone_output_buffer[i]);
			}
		}
		
		function GetUserMediaSuccess(stream)
		{
			NativeMicrophoneLibModule.audioContext = new AudioContext();
			NativeMicrophoneLibModule.microphone_stream = NativeMicrophoneLibModule.audioContext.createMediaStreamSource(stream);
			NativeMicrophoneLibModule.script_processor_node = NativeMicrophoneLibModule.audioContext.createScriptProcessor(NativeMicrophoneLibModule.BUFFER_SIZE, 1, 1);	
			NativeMicrophoneLibModule.script_processor_node.onaudioprocess = MicrophoneProcess;
				
			NativeMicrophoneLibModule.microphone_stream.connect(NativeMicrophoneLibModule.script_processor_node);
		}
		
		function GetUserMediaFailed(value)
		{
			console.log('Record Failed with message: ' + value);		
		}
		
		function StopRecord()
		{
			NativeMicrophoneLibModule.microphone_stream.disconnect(NativeMicrophoneLibModule.script_processor_node);
		}
},
};

autoAddDeps(UnityWebGLNativeLibrary, '$MicrophoneWebGLModuleNative');
mergeInto(LibraryManager.library, UnityWebGLNativeLibrary);