package com.aoihosizora.ncmlatwclient

import android.content.Context
import android.view.View
import android.view.ViewGroup
import android.widget.ArrayAdapter
import android.widget.TextView
import android.widget.Toast
import androidx.annotation.LayoutRes

class EventArrayAdapter(context: Context, @LayoutRes resource: Int, objects: Array<String>) : ArrayAdapter<String>(context, resource, objects) {

    private var mOnItemLongClickListener: ((position: Int, text: String?) -> Boolean)? = null

    fun setOnItemLongPressedListener(listener: ((Int, String?) -> Boolean)?) {
        mOnItemLongClickListener = listener
    }

    override fun getView(position: Int, convertView: View?, parent: ViewGroup): View {
        val v = super.getView(position, convertView, parent)
        val item = getItem(position)
        v.setOnLongClickListener {
            mOnItemLongClickListener?.invoke(position, item) ?: false
        }
        return v
    }
}
