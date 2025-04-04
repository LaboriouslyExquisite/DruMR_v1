import mido
import socket
import time

UDP_IP = "10.0.233.176"  # Replace with your Quest IP (or "127.0.0.1" for local test)
UDP_PORT = 5055

sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)

# Auto-detect AerodrumPort or VMPK
port_name = None
for name in mido.get_input_names():
    if "AerodrumPort" in name or "VMPK" in name:
        port_name = name
        break

if not port_name:
    raise Exception("No MIDI input port found.")

print(f"Using MIDI port: {port_name}")

try:
    with mido.open_input(port_name) as inport:
        print(f"Streaming to {UDP_IP}:{UDP_PORT}")
        for msg in inport:
            if msg.type == 'note_on':
                timestamp = time.time()
                print(f"Note: {msg.note}, Velocity: {msg.velocity}, Time: {timestamp}")
                sock.sendto(b"trigger", (UDP_IP, UDP_PORT))
except KeyboardInterrupt:
    print("Stopped.")
    sock.close()

