package com.aoihosizora.neteasem2dserver;

import android.support.annotation.WorkerThread;

import java.io.PrintWriter;
import java.net.Socket;

class ClientSendUtil {

    static Socket socket;

    @WorkerThread
    public static void sendMsg(String message) {
        try {
            PrintWriter writer = new PrintWriter(socket.getOutputStream(), true);
            writer.println(message);
        }
        catch (Exception ex) {
            ex.getStackTrace();
        }
    }
}
