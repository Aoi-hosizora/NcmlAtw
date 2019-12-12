package com.aoihosizora.neteasem2dclient;

import android.support.annotation.WorkerThread;

import java.io.PrintWriter;
import java.net.Socket;

class SendServer {

    static String ip;
    static int port;

    @WorkerThread
    static boolean ping() {
        try {
            Socket socket = new Socket(ip, port);
            socket.close();
        }
        catch (Exception ex) {
            ex.printStackTrace();
            return false;
        }
        return true;
    }

    @WorkerThread
    static boolean sendMsg(String message) {
        try {
            Socket socket = new Socket(ip, port);
            PrintWriter writer = new PrintWriter(socket.getOutputStream(), true);
            writer.println(message);
            socket.close();
            return true;
        }
        catch (Exception ex) {
            ex.getStackTrace();
            return false;
        }
    }
}