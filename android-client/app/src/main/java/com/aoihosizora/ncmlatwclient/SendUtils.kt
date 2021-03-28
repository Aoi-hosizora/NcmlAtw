package com.aoihosizora.ncmlatwclient

import android.text.TextUtils
import android.util.Patterns
import androidx.annotation.WorkerThread
import java.io.BufferedReader
import java.io.InputStreamReader
import java.io.PrintWriter
import java.lang.Exception
import java.net.InetSocketAddress
import java.net.Socket

object SendUtils {
    var ip: String? = null
    var port: Int? = null
    private const val TIMEOUT = 3000 // 3s

    fun checkIPString(s: String): Boolean {
        return Patterns.IP_ADDRESS.matcher(s).matches()
    }

    fun checkPortString(s: String): Boolean {
        val p: Int
        try {
            p = Integer.parseInt(s)
        } catch (ex: Exception) {
            return false
        }
        if (p < 1 || p > 65535) {
            return false
        }
        return true
    }

    /**
     * Establish socket test connection.
     */
    @WorkerThread
    fun ping(): Boolean {
        if (ip == null || port == null) {
            return true
        }

        val socket = Socket()
        try {
            val addr = InetSocketAddress(ip!!, port!!)
            socket.connect(addr, TIMEOUT)

            val writer = PrintWriter(socket.getOutputStream(), true)
            val reader = BufferedReader(InputStreamReader(socket.getInputStream()))

            writer.println("ping")
            if (reader.readText() == "pong") {
                return true // ping-pong
            }
        } catch (ex: Exception) {
        } finally {
            socket.close()
        }
        return false
    }

    /**
     * Establish connection and send text, return false if use invalid address or fail to send.
     */
    @WorkerThread
    fun send(text: String): Boolean {
        if (ip == null || port == null) {
            return false
        }
        if (TextUtils.isEmpty(text)) {
            return true
        }

        val socket = Socket()
        try {
            val addr = InetSocketAddress(ip!!, port!!)
            socket.connect(addr, TIMEOUT)

            val writer = PrintWriter(socket.getOutputStream(), true)
            writer.println(text)
        } catch (ex: Exception) {
            return false
        } finally {
            socket.close()
        }
        return true
    }
}
