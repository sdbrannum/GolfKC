<script setup lang="ts"></script>

<template>
    <li
        class="border border-slate-300 shadow rounded-md p-2"
        style="min-width: 120px"
    >
        <div class="animate-pulse flex space-x-4">
            <div class="flex-1 space-y-6 py-1">
                <div class="h-2 bg-slate-600 rounded"></div>
                <div class="space-y-3">
                    <div class="grid grid-cols-3 gap-4">
                        <div class="h-2 bg-slate-600 rounded col-span-1"></div>
                        <div class="h-2 rounded col-span-2"></div>
                    </div>
                    <div class="grid grid-cols-3 gap-4">
                        <div class="h-2 bg-slate-600 rounded col-span-1"></div>
                        <div class="h-2 rounded col-span-1"></div>
                        <div class="h-2 bg-slate-600 rounded col-span-1"></div>
                    </div>
                </div>
            </div>
        </div>
    </li>
</template>

<style scoped></style>
