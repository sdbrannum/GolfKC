<script setup lang="ts">
// import { computed } from "vue";
// import { FontAwesomeIcon } from "@fortawesome/vue-fontawesome";
// import { faMap } from "@fortawesome/free-regular-svg-icons";
// import { faUpRightFromSquare } from "@fortawesome/free-solid-svg-icons";
import TeeTimeList from "./TeeTimeList.vue";

type CourseCardProps = {
    id: string;
    name: string;
    source: string;
    uri: string;
    address: Address;
    photo?: string;
    date: string | null;
};
defineProps<CourseCardProps>();

// const mapUri = computed(() => {
//     const query = encodeURIComponent(
//         `${props.name} ${props.address.city} ${props.address.state}`
//     );
//     return `http://maps.google.com?q=${query}`;
// });
</script>

<template>
    <article class="w-full p-4 mb-2 rounded border-2 border-slate-800 relative">
        <a
            class="ml-auto text-rose-500 font-bold text-sm"
            target="_blank"
            :href="uri"
            aria-label="Book tee time"
        >
            {{ name }}
        </a>
        <p class="text-xs">{{ address.city }}, {{ address.state }}</p>
        <tee-time-list :course-id="id" :source="source" :date="date" />

        <!--    <a target="_blank" :href="mapUri">-->
        <!--      Directions-->
        <!--      <font-awesome-icon :icon="faMap"/>-->
        <!--    </a>-->
    </article>
</template>

<style scoped></style>
