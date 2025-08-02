<p align="center">
  <img src="https://i.postimg.cc/QtghNf7w/icon.png" width="80" alt="SSH-Direct-Client Icon">
</p>
<h1 align="center">SSH-Direct-Client</h1>

<div style="display: flex; gap: 10px; flex-wrap: wrap;">
    <img src="https://img.shields.io/github/license/arvinesmaeily/SSHDirectClient" alt="License">
    <img src="https://img.shields.io/github/last-commit/arvinesmaeily/SSHDirectClient" alt="Last Commit">
    <img src="https://img.shields.io/github/issues/arvinesmaeily/SSHDirectClient" alt="Open Issues">
</div>
<br/>


A WPF-based Windows application that creates a secure SOCKS5 proxy tunnel to a remote server. It's designed to be a simple, graphical alternative to the `ssh -D` or `ssh -L` command.

This project utilizes the excellent [SSH.NET](https://github.com/sshnet/SSH.NET) library.

## ✨ Features

* **Profile Management**: Save and manage multiple SSH server configurations.
* **One-Click Connection**: Easily connect or disconnect with a single button.
* **Visual Status Indicator**: Instantly know your connection status with color-coded feedback.
* **Customizable Proxy**: Configure the local SOCKS5 IP address and port.
* **Light & Dark Themes**: Match the app to your system's theme.
* **Connection Logs**: A dedicated panel for real-time logs to troubleshoot connection issues.

---

## 🚀 Getting Started

Follow these steps to get the client up and running.

### Prerequisites

* **Operating System**: Windows 10 (version 20H1 or newer) / Windows 11 is recommended. Feel free to test other versions.
* **Display**: Make sure your display resolution is high enough. (Higher than 1280*720 is recommended.)

### Installation

1.  Navigate to the [**Releases**](https://github.com/arvinesmaeily/SSHDirectClient/releases) page of this repository.
2.  Download the latest release.
3.  Install the software.
4.  Run Desktop shortcut of SSH-Direct Client.

---

## 🔧 Usage Guide

The application interface is designed to be intuitive, with sections arranged from top to bottom.

### 1. Server Configuration

Before connecting, you need to add a server profile.

1.  Click **Edit Configurations**.
2.  In the new window, provide the following details:
    * **Name**: A friendly name for your configuration (e.g., "Work Server").
    * **Host Address**: The IP address or domain of the SSH server (e.g., `123.123.123.123`).
    * **Host Port**: The SSH port on the server. The default is `22`.
    * **Username & Password**: Your credentials for the SSH server.
3.  Click **Add** to save the new profile. You can also select a profile from the list to **Edit** or **Delete** it.

### 2. Connection Button

The main button serves to connect/disconnect and shows the current status:

* **🟢 Spinning**: Connecting to the host.
* **🟢 Solid**: Successfully connected.
* **🟡 Spinning**: An error occurred; attempting to reconnect.
* **🔴 Solid**: Disconnected due to an unrecoverable error.

**NOTE**: Make sure to select a profile from the list, before clicking on connect!

### 3. SOCKS5 Properties

This section controls the local proxy created by the tunnel. Other applications on your PC can be configured to use this proxy.

* **SOCKS5 IP Address**: Defaults to `127.0.0.1` (localhost).
* **SOCKS5 Port**: Defaults to `1080`.

You can change these values if the default address is already in use or if you have specific networking needs.

### 4. SSH Connection Properties

Here you can adjust advanced SSH parameters like the Keep-Alive interval. **It is recommended to leave these at their default values** unless you are an advanced user and know why you need to change them.

### 5. Logs

This panel displays real-time output from the SSH connection. It is essential for **debugging** any connectivity problems. If you encounter an issue, the information here will be very helpful.

---

## 🐞 Reporting Issues

If you encounter a bug or have a suggestion, please [open an issue](https://github.com/arvinesmaeily/SSHDirectClient/issues) on the repository.

When reporting a connection problem, please **include the relevant output from the Logs panel** to help us diagnose the issue faster.

## License

This work is under an [MIT](https://choosealicense.com/licenses/mit/) License. Visit [License](https://github.com/arvinesmaeily/SSHDirectClient/blob/master/LICENSE) for more info.
